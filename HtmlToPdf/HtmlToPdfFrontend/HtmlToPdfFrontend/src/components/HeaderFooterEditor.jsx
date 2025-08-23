import React from "react";

const HeaderFooterEditor = ({ type, config, onChange }) => {
  const isHeader = type === 'header';
  const title = isHeader ? 'Header' : 'Footer';
  
  const fonts = [
    'Arial', 'Times New Roman', 'Helvetica', 'Georgia', 'Verdana', 
    'Calibri', 'Tahoma', 'Trebuchet MS'
  ];
  
  const alignments = [
    { value: 'left', label: 'Left', icon: '⬅️' },
    { value: 'center', label: 'Center', icon: '↔️' },
    { value: 'right', label: 'Right', icon: '➡️' }
  ];

  const fontSizes = [6, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36];

  const handleChange = (field, value) => {
    onChange({
      ...config,
      [field]: value
    });
  };

  return (
    <div className="space-y-6">
      <h3 className="text-lg font-medium text-white">{title} Settings</h3>
      
      <div className="space-y-4">
        
        {/* Header/Footer Text */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            {title} Text
          </label>
          <textarea
            value={config?.text || ""}
            placeholder={`Enter ${title.toLowerCase()} content...`}
            onChange={(e) => handleChange('text', e.target.value)}
            rows={3}
            className="w-full px-3 py-2 bg-[#0D1164]/30 border border-gray-300/30 rounded-lg text-white placeholder-gray-300 resize-none focus:outline-none focus:ring-2 focus:ring-[#F78D60]/50"
          />
          <p className="text-xs text-gray-400 mt-1">
            Leave empty to disable {title.toLowerCase()}
          </p>
        </div>

        {/* Font Settings - Only show if text is provided */}
        {config?.text && (
          <>
            {/* Font Family */}
            <div>
              <label className="block text-sm font-medium text-white mb-2">
                Font Family
              </label>
              <div className="relative">
                <select
                  value={config?.font || 'Arial'}
                  onChange={(e) => handleChange('font', e.target.value)}
                  className="w-full p-3 bg-[#0D1164]/30 border border-gray-300/30 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-[#F78D60]/50 appearance-none pr-10"
                >
                  {fonts.map((font) => (
                    <option key={font} value={font} className="bg-[#0D1164] text-white">
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
            </div>

            {/* Font Size */}
            <div>
              <label className="block text-sm font-medium text-white mb-2">
                Font Size
              </label>
              <div className="flex items-center space-x-3">
                <div className="relative flex-1">
                  <select
                    value={config?.fontSize || 10}
                    onChange={(e) => handleChange('fontSize', parseInt(e.target.value))}
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
                    onClick={() => handleChange('fontSize', Math.max(6, (config?.fontSize || 10) - 1))}
                    className="p-2 bg-[#640D5F]/30 hover:bg-[#640D5F]/50 text-white rounded-lg transition-colors"
                    title="Decrease font size"
                  >
                    <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clipRule="evenodd" />
                    </svg>
                  </button>
                  <button
                    onClick={() => handleChange('fontSize', Math.min(36, (config?.fontSize || 10) + 1))}
                    className="p-2 bg-[#640D5F]/30 hover:bg-[#640D5F]/50 text-white rounded-lg transition-colors"
                    title="Increase font size"
                  >
                    <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clipRule="evenodd" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            {/* Alignment */}
            <div>
              <label className="block text-sm font-medium text-white mb-2">
                Text Alignment
              </label>
              <div className="grid grid-cols-3 gap-2">
                {alignments.map((alignment) => (
                  <button
                    key={alignment.value}
                    onClick={() => handleChange('alignment', alignment.value)}
                    className={`p-3 rounded-lg border transition-colors ${
                      (config?.alignment || 'left') === alignment.value
                        ? 'border-[#F78D60] bg-[#F78D60]/20 text-white'
                        : 'border-gray-300/30 bg-[#0D1164]/20 text-gray-300 hover:border-[#F78D60]/50'
                    }`}
                  >
                    <span className="block text-lg mb-1">{alignment.icon}</span>
                    <span className="text-sm">{alignment.label}</span>
                  </button>
                ))}
              </div>
            </div>

            {/* Preview */}
            <div>
              <label className="block text-sm font-medium text-white mb-2">
                Preview
              </label>
              <div className="p-4 bg-white rounded-lg border">
                <div 
                  style={{
                    fontFamily: config?.font || 'Arial',
                    fontSize: `${config?.fontSize || 10}px`,
                    textAlign: config?.alignment || 'left',
                    color: '#333'
                  }}
                >
                  {config?.text || `Sample ${title.toLowerCase()} text`}
                </div>
              </div>
            </div>
          </>
        )}

        {/* Reset Button */}
        <div className="pt-4 border-t border-white/20">
          <button
            onClick={() => onChange({
              text: '',
              font: 'Arial',
              fontSize: 10,
              alignment: 'left'
            })}
            className="text-sm text-gray-300 hover:text-[#F78D60] transition-colors"
          >
            Reset {title} Settings
          </button>
        </div>
      </div>
    </div>
  );
};

export default HeaderFooterEditor;
