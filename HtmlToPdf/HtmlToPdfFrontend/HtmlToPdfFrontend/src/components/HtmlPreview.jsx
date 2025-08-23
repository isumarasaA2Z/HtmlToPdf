import { useEffect, useRef } from 'react'

const HtmlPreview = ({ htmlContent, customizations }) => {
  const iframeRef = useRef(null)

  useEffect(() => {
    if (iframeRef.current && htmlContent) {
      const iframe = iframeRef.current
      const doc = iframe.contentDocument || iframe.contentWindow.document
      
      // Apply customizations to the HTML content
      const styledHtml = applyCustomizations(htmlContent, customizations)
      
      doc.open()
      doc.write(styledHtml)
      doc.close()
    }
  }, [htmlContent, customizations])

  const applyCustomizations = (html, custom) => {
    if (!html) return ''
    
    // Inject custom styles
    const customStyles = `
      <style>
        body {
          font-family: ${custom.fontFamily}, sans-serif !important;
          font-size: ${custom.fontSize}px !important;
          margin: ${custom.margins.top}mm ${custom.margins.right}mm ${custom.margins.bottom}mm ${custom.margins.left}mm !important;
          background: white;
          color: #333;
        }
        
        .preview-header {
          position: fixed;
          top: 0;
          left: 0;
          right: 0;
          background: #f8f9fa;
          border-bottom: 1px solid #dee2e6;
          padding: 10px 20px;
          font-family: ${custom.header.font}, sans-serif;
          font-size: ${custom.header.fontSize}px;
          text-align: ${custom.header.alignment};
          z-index: 1000;
        }
        
        .preview-footer {
          position: fixed;
          bottom: 0;
          left: 0;
          right: 0;
          background: #f8f9fa;
          border-top: 1px solid #dee2e6;
          padding: 10px 20px;
          font-family: ${custom.footer.font}, sans-serif;
          font-size: ${custom.footer.fontSize}px;
          text-align: ${custom.footer.alignment};
          z-index: 1000;
        }
        
        .content-wrapper {
          margin-top: ${custom.header.text ? '60px' : '0'};
          margin-bottom: ${custom.footer.text ? '60px' : '0'};
          padding: 20px;
        }
      </style>
    `
    
    // Wrap content with header/footer
    const wrappedHtml = `
      <!DOCTYPE html>
      <html>
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        ${customStyles}
      </head>
      <body>
        ${custom.header.text ? `<div class="preview-header">${custom.header.text}</div>` : ''}
        <div class="content-wrapper">
          ${html}
        </div>
        ${custom.footer.text ? `<div class="preview-footer">${custom.footer.text}</div>` : ''}
      </body>
      </html>
    `
    
    return wrappedHtml
  }

  const mockHtmlContent = `
    <h1>Purchase Order Quotation</h1>
    <p>This Purchase Order <strong>PO-2024-001</strong> contains the following data:</p>
    
    <table style="width: 100%; border-collapse: collapse; margin-top: 20px;">
        <tr>
            <th style="border: 1px solid #ddd; padding: 8px; text-align: left; background-color: #f4f4f4;">Order No</th>
            <th style="border: 1px solid #ddd; padding: 8px; text-align: left; background-color: #f4f4f4;">Line No</th>
            <th style="border: 1px solid #ddd; padding: 8px; text-align: left; background-color: #f4f4f4;">Part No</th>
            <th style="border: 1px solid #ddd; padding: 8px; text-align: left; background-color: #f4f4f4;">Quantity</th>
        </tr>
        <tr>
            <td style="border: 1px solid #ddd; padding: 8px;">PO-001</td>
            <td style="border: 1px solid #ddd; padding: 8px;">1</td>
            <td style="border: 1px solid #ddd; padding: 8px;">PART-A123</td>
            <td style="border: 1px solid #ddd; padding: 8px;">100</td>
        </tr>
        <tr>
            <td style="border: 1px solid #ddd; padding: 8px;">PO-001</td>
            <td style="border: 1px solid #ddd; padding: 8px;">2</td>
            <td style="border: 1px solid #ddd; padding: 8px;">PART-B456</td>
            <td style="border: 1px solid #ddd; padding: 8px;">50</td>
        </tr>
    </table>
    
    <h2>Delivery Locations:</h2>
    <ul>
        <li>No: 5, Jayamalapauara, Gampola</li>
        <li>No: 10 Kandy Road, Peradeniya</li>
        <li>No: 55 KCC, Kandy</li>
    </ul>

    <p>Order Reference: <strong>REF-12345</strong></p>
  `

  return (
    <div className="bg-white/10 backdrop-blur-md rounded-xl border border-white/20 p-6 h-full flex flex-col">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold text-white">HTML Preview</h2>
        <div className="flex items-center space-x-2">
          <span className="text-sm text-gray-300">
            {customizations.pageSize} • {customizations.orientation}
          </span>
          <div className="w-3 h-3 bg-green-400 rounded-full"></div>
        </div>
      </div>
      
      <div className="flex-1 bg-white rounded-lg border-2 border-gray-200 overflow-hidden">
        <iframe
          ref={iframeRef}
          className="w-full h-full border-none"
          title="HTML Preview"
          sandbox="allow-same-origin"
        />
      </div>
      
      {!htmlContent && (
        <div className="absolute inset-0 flex items-center justify-center bg-white/5 rounded-lg">
          <div className="text-center text-gray-300">
            <div className="w-16 h-16 mx-auto mb-4 rounded-lg bg-[#640D5F]/20 flex items-center justify-center">
              <svg className="w-8 h-8" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zm0 4a1 1 0 011-1h12a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1V8z" clipRule="evenodd" />
              </svg>
            </div>
            <p className="text-lg font-medium">No HTML content</p>
            <p className="text-sm text-gray-400 mt-1">Enter JSON data to see preview</p>
          </div>
        </div>
      )}
    </div>
  )
}

export default HtmlPreview
