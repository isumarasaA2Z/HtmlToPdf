/**
 * Utility functions to transform frontend data to backend Report structure
 */

/**
 * Transform frontend customizations to backend PageSetup format
 * @param {Object} customizations - Frontend customizations object
 * @returns {Object} - Backend PageSetup object
 */
export const transformToPageSetup = (customizations) => {
  return {
    size: customizations.pageSize || 'A4',
    orientation: customizations.orientation || 'portrait',
    fontFamily: customizations.fontFamily || 'Arial',
    pageMargin: {
      headerMargin: {
        left: customizations.margins?.left || 20,
        right: customizations.margins?.right || 20,
        height: customizations.margins?.top || 20
      },
      footerMargin: {
        left: customizations.margins?.left || 20,
        right: customizations.margins?.right || 20,
        height: customizations.margins?.bottom || 20
      }
    },
    headerText: {
      text: customizations.header?.text || '',
      font: customizations.header?.font || 'Arial',
      fontSize: customizations.header?.fontSize || 10,
      alignment: customizations.header?.alignment || 'left'
    },
    footerText: {
      text: customizations.footer?.text || '',
      font: customizations.footer?.font || 'Arial',
      fontSize: customizations.footer?.fontSize || 10,
      alignment: customizations.footer?.alignment || 'left'
    }
  };
};

/**
 * Transform frontend data to complete backend Report structure
 * @param {string} jsonData - Raw JSON string from frontend
 * @param {Object} customizations - Frontend customizations
 * @returns {Object} - Complete Report object for backend
 */
export const transformToReport = (jsonData, customizations) => {
  let parsedData = {};
  
  try {
    parsedData = JSON.parse(jsonData);
  } catch (error) {
    throw new Error('Invalid JSON data provided');
  }

  // Extract texts and tables from parsed JSON, or use defaults
  const texts = parsedData.reportData?.texts || parsedData.texts || [];
  const tables = parsedData.reportData?.tables || parsedData.tables || [];

  // Transform to backend structure
  const report = {
    customerId: parsedData.customerId || null,
    requestId: parsedData.requestId || generateRequestId(),
    reportData: {
      includePageNumber: true,
      pageSetup: transformToPageSetup(customizations),
      texts: texts.map(text => ({
        name: text.name,
        value: text.value
      })),
      images: tables.map(table => ({
        name: table.name,
        headers: table.headers || [],
        rows: table.rows || [],
        tableMetaData: table.tableMetaData || '',
        headerRowMetaData: table.headerRowMetaData || '',
        headerCellMetaData: table.headerCellMetaData || '',
        rowMetaData: table.rowMetaData || '',
        cellMetaData: table.cellMetaData || ''
      }))
    }
  };

  return report;
};

/**
 * Generate a unique request ID
 * @returns {string} - Unique request ID
 */
const generateRequestId = () => {
  return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Validate frontend customizations before transformation
 * @param {Object} customizations - Frontend customizations
 * @returns {Object} - Validation result with isValid and errors
 */
export const validateCustomizations = (customizations) => {
  const errors = [];

  if (!customizations) {
    errors.push('Customizations object is required');
    return { isValid: false, errors };
  }

  // Validate margins
  if (customizations.margins) {
    const { top, right, bottom, left } = customizations.margins;
    if (isNaN(top) || top < 0 || top > 100) errors.push('Top margin must be between 0-100mm');
    if (isNaN(right) || right < 0 || right > 100) errors.push('Right margin must be between 0-100mm');
    if (isNaN(bottom) || bottom < 0 || bottom > 100) errors.push('Bottom margin must be between 0-100mm');
    if (isNaN(left) || left < 0 || left > 100) errors.push('Left margin must be between 0-100mm');
  }

  // Validate font size
  if (customizations.fontSize && (isNaN(customizations.fontSize) || customizations.fontSize < 8 || customizations.fontSize > 72)) {
    errors.push('Font size must be between 8-72px');
  }

  // Validate header font size
  if (customizations.header?.fontSize && (isNaN(customizations.header.fontSize) || customizations.header.fontSize < 6 || customizations.header.fontSize > 36)) {
    errors.push('Header font size must be between 6-36px');
  }

  // Validate footer font size
  if (customizations.footer?.fontSize && (isNaN(customizations.footer.fontSize) || customizations.footer.fontSize < 6 || customizations.footer.fontSize > 36)) {
    errors.push('Footer font size must be between 6-36px');
  }

  return {
    isValid: errors.length === 0,
    errors
  };
};

/**
 * Validate JSON data structure
 * @param {string} jsonData - JSON string to validate
 * @returns {Object} - Validation result
 */
export const validateJsonData = (jsonData) => {
  const errors = [];

  if (!jsonData || jsonData.trim() === '') {
    errors.push('JSON data is required');
    return { isValid: false, errors };
  }

  try {
    const parsed = JSON.parse(jsonData);

    // Check for required structure
    if (!parsed.reportData && !parsed.texts && !parsed.tables) {
      errors.push('JSON must contain either "reportData" or "texts"/"tables" properties');
    }

    // Validate texts structure if present
    const texts = parsed.reportData?.texts || parsed.texts;
    if (texts && Array.isArray(texts)) {
      texts.forEach((text, index) => {
        if (!text.name || !text.value) {
          errors.push(`Text item at index ${index} must have "name" and "value" properties`);
        }
      });
    }

    // Validate tables structure if present
    const tables = parsed.reportData?.tables || parsed.tables;
    if (tables && Array.isArray(tables)) {
      tables.forEach((table, index) => {
        if (!table.name) {
          errors.push(`Table at index ${index} must have a "name" property`);
        }
        if (!Array.isArray(table.headers)) {
          errors.push(`Table at index ${index} must have a "headers" array`);
        }
        if (!Array.isArray(table.rows)) {
          errors.push(`Table at index ${index} must have a "rows" array`);
        }
      });
    }

  } catch (error) {
    errors.push(`Invalid JSON format: ${error.message}`);
  }

  return {
    isValid: errors.length === 0,
    errors
  };
};
