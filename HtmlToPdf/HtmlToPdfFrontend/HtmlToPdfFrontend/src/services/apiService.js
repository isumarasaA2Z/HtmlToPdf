// API service for backend communication
const API_BASE_URL = 'https://localhost:7050'; // .NET API HTTPS port from launchSettings.json

class ApiService {
  
  /**
   * Convert JSON data to PDF using backend API
   * @param {Object} reportData - The report data matching backend Report structure
   * @returns {Promise<Blob>} - PDF blob
   */
  async convertToPdf(reportData) {
    try {
      const response = await fetch(`${API_BASE_URL}/api/convert-to_pdf`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(reportData)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP ${response.status}: ${errorText}`);
      }

      // Return PDF as blob
      const pdfBlob = await response.blob();
      return pdfBlob;
      
    } catch (error) {
      console.error('API Error:', error);
      throw new Error(`Failed to generate PDF: ${error.message}`);
    }
  }

  /**
   * Get filled HTML content (if you want to add this endpoint later)
   * @param {Object} jsonData - Raw JSON data
   * @returns {Promise<string>} - Filled HTML content
   */
  async getFilledHtml(jsonData) {
    try {
      // This would be a separate endpoint to just get filled HTML
      const response = await fetch(`${API_BASE_URL}/api/fill-html`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(jsonData)
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const htmlContent = await response.text();
      return htmlContent;
      
    } catch (error) {
      console.error('API Error:', error);
      throw new Error(`Failed to get HTML: ${error.message}`);
    }
  }

  /**
   * Health check for backend
   * @returns {Promise<boolean>} - True if backend is available
   */
  async healthCheck() {
    try {
      const response = await fetch(`${API_BASE_URL}/health`, {
        method: 'GET',
        timeout: 5000
      });
      return response.ok;
    } catch (error) {
      console.error('Backend health check failed:', error);
      return false;
    }
  }
}

export default new ApiService();
