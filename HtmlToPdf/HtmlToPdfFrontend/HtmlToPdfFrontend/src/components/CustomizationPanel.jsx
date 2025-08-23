import { useState } from 'react'
import FontSelector from './FontSelector'
import MarginSelector from './MarginSelector'
import HeaderFooterEditor from './HeaderFooterEditor'

const CustomizationPanel = ({ customizations, onChange, onGeneratePdf, isGenerating, backendStatus }) => {
  const [activeTab, setActiveTab] = useState('general')
  
  const tabs = [
    { id: 'general', name: 'General', icon: '⚙️' },
    { id: 'header', name: 'Header', icon: '📄' },
    { id: 'footer', name: 'Footer', icon: '📝' },
    { id: 'margins', name: 'Margins', icon: '📐' }
  ]

  const pageSizes = ['A3', 'A4', 'A5', 'Letter', 'Legal', 'Tabloid', 'Ledger']
  const orientations = [
    { value: 'portrait', label: 'Portrait', icon: '📱' },
    { value: 'landscape', label: 'Landscape', icon: '💻' }
  ]

  return (
    <div className="bg-white/10 backdrop-blur-md rounded-xl border border-white/20 p-6 h-full flex flex-col">
      {/* Header */}
      <div className="mb-6">
        <h2 className="text-xl font-semibold text-white mb-2">Customization</h2>
        <div className="flex space-x-1 bg-[#640D5F]/30 rounded-lg p-1">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex-1 px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                activeTab === tab.id
                  ? 'bg-[#F78D60] text-white'
                  : 'text-gray-300 hover:text-white hover:bg-white/10'
              }`}
            >
              <span className="mr-1">{tab.icon}</span>
              {tab.name}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      <div className="flex-1 overflow-y-auto">
        
        {/* General Tab */}
        {activeTab === 'general' && (
          <div className="space-y-6">
            
            {/* Font Settings */}
            <div>
              <h3 className="text-lg font-medium text-white mb-3">Font Settings</h3>
              <FontSelector
                fontFamily={customizations.fontFamily}
                fontSize={customizations.fontSize}
                onFontFamilyChange={(font) => onChange('fontFamily', font)}
                onFontSizeChange={(size) => onChange('fontSize', size)}
              />
            </div>

            {/* Page Settings */}
            <div>
              <h3 className="text-lg font-medium text-white mb-3">Page Settings</h3>
              
              {/* Page Size */}
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-300 mb-2">
                  Page Size
                </label>
                <select
                  value={customizations.pageSize}
                  onChange={(e) => onChange('pageSize', e.target.value)}
                  className="w-full p-3 bg-[#0D1164]/30 border border-gray-300/30 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-[#F78D60]/50"
                >
                  {pageSizes.map((size) => (
                    <option key={size} value={size} className="bg-[#0D1164] text-white">
                      {size}
                    </option>
                  ))}
                </select>
              </div>

              {/* Orientation */}
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">
                  Orientation
                </label>
                <div className="grid grid-cols-2 gap-2">
                  {orientations.map((orientation) => (
                    <button
                      key={orientation.value}
                      onClick={() => onChange('orientation', orientation.value)}
                      className={`p-3 rounded-lg border transition-colors ${
                        customizations.orientation === orientation.value
                          ? 'border-[#F78D60] bg-[#F78D60]/20 text-white'
                          : 'border-gray-300/30 bg-[#0D1164]/20 text-gray-300 hover:border-[#F78D60]/50'
                      }`}
                    >
                      <span className="block text-lg mb-1">{orientation.icon}</span>
                      <span className="text-sm">{orientation.label}</span>
                    </button>
                  ))}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Header Tab */}
        {activeTab === 'header' && (
          <HeaderFooterEditor
            type="header"
            config={customizations.header}
            onChange={(newConfig) => onChange('header', newConfig)}
          />
        )}

        {/* Footer Tab */}
        {activeTab === 'footer' && (
          <HeaderFooterEditor
            type="footer"
            config={customizations.footer}
            onChange={(newConfig) => onChange('footer', newConfig)}
          />
        )}

        {/* Margins Tab */}
        {activeTab === 'margins' && (
          <MarginSelector
            margins={customizations.margins}
            onChange={(newMargins) => onChange('margins', newMargins)}
          />
        )}

      </div>

      {/* Generate PDF Button */}
      <div className="mt-6 pt-6 border-t border-white/20">
        <button
          onClick={onGeneratePdf}
          disabled={isGenerating || backendStatus === 'offline'}
          className={`w-full font-semibold py-4 px-6 rounded-lg transition-all duration-200 transform shadow-lg ${
            isGenerating || backendStatus === 'offline'
              ? 'bg-gray-500 cursor-not-allowed opacity-50'
              : 'bg-gradient-to-r from-[#EA2264] to-[#F78D60] hover:from-[#EA2264]/80 hover:to-[#F78D60]/80 text-white hover:scale-[1.02] active:scale-[0.98]'
          }`}
        >
          <span className="flex items-center justify-center">
            {isGenerating ? (
              <>
                <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Generating PDF...
              </>
            ) : backendStatus === 'offline' ? (
              <>
                <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
                Backend Offline
              </>
            ) : (
              <>
                <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1v-2zM3.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0L9 3.586V12a1 1 0 102 0V3.586l1.293-1.293a1 1 0 011.414 0l3 3a1 1 0 010 1.414L15.414 8l-1.707 1.707A1 1 0 0112 9V12a1 1 0 11-2 0V9a1 1 0 01-1.707.707L6.586 8 3.293 6.707z" clipRule="evenodd" />
                </svg>
                Generate PDF
              </>
            )}
          </span>
        </button>
        
        {/* Backend Status Help Text */}
        {backendStatus === 'offline' && (
          <p className="text-xs text-red-300 mt-2 text-center">
            Backend service is offline. Please ensure the .NET API is running on localhost:5000
          </p>
        )}
        
        {isGenerating && (
          <p className="text-xs text-gray-300 mt-2 text-center">
            Please wait while your PDF is being generated...
          </p>
        )}
      </div>
    </div>
  )
}

export default CustomizationPanel
