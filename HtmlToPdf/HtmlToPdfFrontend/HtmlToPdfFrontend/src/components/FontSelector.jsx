const FontSelector = ({ fontFamily, fontSize, onFontFamilyChange, onFontSizeChange }) => {
  const fonts = [
    'Arial',
    'Times New Roman',
    'Helvetica',
    'Georgia',
    'Verdana',
    'Calibri',
    'Tahoma',
    'Trebuchet MS',
    'Impact',
    'Comic Sans MS'
  ]

  const fontSizes = [8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72]

  return (
    <div className="space-y-4">
      {/* Font Family */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Font Family
        </label>
        <div className="relative">
          <select
            value={fontFamily}
            onChange={(e) => onFontFamilyChange(e.target.value)}
            className="w-full p-3 bg-[#0D1164]/30 border border-gray-300/30 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-[#F78D60]/50 appearance-none pr-10"
          >
            {fonts.map((font) => (
              <option 
                key={font} 
                value={font} 
                className="bg-[#0D1164] text-white"
                style={{ fontFamily: font }}
              >
                {font}
              </option>
            ))}
          </select>
          <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
            <svg className="w-5 h-5 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
            </svg>
          </div>
        </div>
        
        {/* Font Preview */}
        <div className="mt-2 p-3 bg-white/5 rounded-lg">
          <p 
            className="text-white text-lg"
            style={{ fontFamily: fontFamily }}
          >
            The quick brown fox jumps over the lazy dog
          </p>
        </div>
      </div>

      {/* Font Size */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Font Size
        </label>
        <div className="flex items-center space-x-3">
          
          {/* Size Dropdown */}
          <div className="relative flex-1">
            <select
              value={fontSize}
              onChange={(e) => onFontSizeChange(parseInt(e.target.value))}
              className="w-full p-3 bg-[#0D1164]/30 border border-gray-300/30 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-[#F78D60]/50 appearance-none pr-10"
            >
              {fontSizes.map((size) => (
                <option key={size} value={size} className="bg-[#0D1164] text-white">
                  {size}px
                </option>
              ))}
            </select>
            <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
              <svg className="w-5 h-5 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
              </svg>
            </div>
          </div>

          {/* Size Buttons */}
          <div className="flex space-x-1">
            <button
              onClick={() => onFontSizeChange(Math.max(8, fontSize - 1))}
              className="p-2 bg-[#640D5F]/30 hover:bg-[#640D5F]/50 text-white rounded-lg transition-colors"
              title="Decrease font size"
            >
              <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clipRule="evenodd" />
              </svg>
            </button>
            <button
              onClick={() => onFontSizeChange(Math.min(72, fontSize + 1))}
              className="p-2 bg-[#640D5F]/30 hover:bg-[#640D5F]/50 text-white rounded-lg transition-colors"
              title="Increase font size"
            >
              <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clipRule="evenodd" />
              </svg>
            </button>
          </div>
        </div>

        {/* Size Preview */}
        <div className="mt-2 p-3 bg-white/5 rounded-lg">
          <p 
            className="text-white"
            style={{ 
              fontFamily: fontFamily, 
              fontSize: `${fontSize}px` 
            }}
          >
            Sample text at {fontSize}px
          </p>
        </div>
      </div>
    </div>
  )
}

export default FontSelector
