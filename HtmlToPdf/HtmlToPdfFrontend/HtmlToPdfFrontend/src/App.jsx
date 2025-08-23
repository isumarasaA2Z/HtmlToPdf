import { useState, useEffect } from 'react'
import JsonInput from './components/JsonInput'
import HtmlPreview from './components/HtmlPreview'
import CustomizationPanel from './components/CustomizationPanel'
import apiService from './services/apiService'
import { transformToReport, validateJsonData, validateCustomizations } from './utils/dataTransform'
import './App.css'

function App() {
  const [jsonData, setJsonData] = useState('')
  const [htmlContent, setHtmlContent] = useState('')
  const [customizations, setCustomizations] = useState({
    fontFamily: 'Arial',
    fontSize: 12,
    margins: {
      top: 20,
      bottom: 20,
      left: 20,
      right: 20
    },
    header: {
      text: '',
      font: 'Arial',
      fontSize: 10,
      alignment: 'left'
    },
    footer: {
      text: '',
      font: 'Arial', 
      fontSize: 10,
      alignment: 'left'
    },
    pageSize: 'A4',
    orientation: 'portrait'
  })

  // Loading and error states
  const [isGeneratingPdf, setIsGeneratingPdf] = useState(false)
  const [error, setError] = useState(null)
  const [backendStatus, setBackendStatus] = useState('unknown') // 'online', 'offline', 'unknown'

  // Check backend health on component mount
  useEffect(() => {
    checkBackendHealth()
  }, [])

  const checkBackendHealth = async () => {
    try {
      const isOnline = await apiService.healthCheck()
      setBackendStatus(isOnline ? 'online' : 'offline')
    } catch (error) {
      setBackendStatus('offline')
    }
  }

  const handleJsonChange = (json) => {
    setJsonData(json)
    setError(null) // Clear previous errors
    
    // TODO: Optionally, you could call backend to get filled HTML here
    // For now, we'll just update the preview with mock content
  }

  const handleCustomizationChange = (key, value) => {
    setCustomizations(prev => ({
      ...prev,
      [key]: value
    }))
    setError(null) // Clear previous errors
  }

  const downloadPdf = (pdfBlob) => {
    // Create download link
    const url = window.URL.createObjectURL(pdfBlob)
    const link = document.createElement('a')
    link.href = url
    link.download = `report_${new Date().toISOString().slice(0, 10)}.pdf`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  }

  const generatePdf = async () => {
    setIsGeneratingPdf(true)
    setError(null)

    try {
      // Validate JSON data
      const jsonValidation = validateJsonData(jsonData)
      if (!jsonValidation.isValid) {
        throw new Error(`Invalid JSON data: ${jsonValidation.errors.join(', ')}`)
      }

      // Validate customizations
      const customValidation = validateCustomizations(customizations)
      if (!customValidation.isValid) {
        throw new Error(`Invalid customizations: ${customValidation.errors.join(', ')}`)
      }

      // Transform data to backend format
      const reportData = transformToReport(jsonData, customizations)
      
      console.log('Sending to backend:', reportData)

      // Call backend API
      const pdfBlob = await apiService.convertToPdf(reportData)
      
      // Download the PDF
      downloadPdf(pdfBlob)

      // Show success message
      setError({ type: 'success', message: 'PDF generated successfully!' })

    } catch (error) {
      console.error('PDF generation failed:', error)
      setError({ 
        type: 'error', 
        message: error.message || 'Failed to generate PDF. Please check your data and try again.' 
      })
    } finally {
      setIsGeneratingPdf(false)
    }
  }

  return (
    <div className="min-h-screen bg-[#EA2264]">
      {/* Header */}
      <header className="bg-[#640D5F]/90 backdrop-blur-sm border-b border-[#640D5F]/30">
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-white">
                HTML to PDF Generator
              </h1>
              <p className="text-gray-300 mt-1">
                Convert JSON data to customized PDF documents
              </p>
            </div>
            
            {/* Backend Status Indicator */}
            <div className="flex items-center space-x-2">
              <div className={`w-3 h-3 rounded-full ${
                backendStatus === 'online' ? 'bg-green-400' : 
                backendStatus === 'offline' ? 'bg-red-400' : 'bg-yellow-400'
              }`}></div>
              <span className="text-sm text-gray-300">
                Backend: {backendStatus === 'online' ? 'Online' : 
                         backendStatus === 'offline' ? 'Offline' : 'Checking...'}
              </span>
              <button
                onClick={checkBackendHealth}
                className="text-xs text-gray-400 hover:text-white transition-colors"
                title="Refresh backend status"
              >
                🔄
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Error/Success Banner */}
      {error && (
        <div className={`border-l-4 p-4 mx-6 mt-4 rounded ${
          error.type === 'success' 
            ? 'bg-green-500/20 border-green-400 text-green-300' 
            : 'bg-red-500/20 border-red-400 text-red-300'
        }`}>
          <div className="flex items-center justify-between">
            <p className="text-sm">{error.message}</p>
            <button 
              onClick={() => setError(null)}
              className="text-lg hover:opacity-70 transition-opacity"
            >
              ×
            </button>
          </div>
        </div>
      )}

      {/* Main Content */}
      <main className="container mx-auto px-6 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 h-[calc(100vh-200px)]">
          
          {/* JSON Input Panel */}
          <div className="lg:col-span-1">
            <JsonInput 
              value={jsonData}
              onChange={handleJsonChange}
            />
          </div>

          {/* HTML Preview Panel */}
          <div className="lg:col-span-1">
            <HtmlPreview 
              htmlContent={htmlContent}
              customizations={customizations}
            />
          </div>

          {/* Customization Panel */}
          <div className="lg:col-span-1">
            <CustomizationPanel
              customizations={customizations}
              onChange={handleCustomizationChange}
              onGeneratePdf={generatePdf}
              isGenerating={isGeneratingPdf}
              backendStatus={backendStatus}
            />
          </div>

        </div>
      </main>
    </div>
  )
}

export default App
