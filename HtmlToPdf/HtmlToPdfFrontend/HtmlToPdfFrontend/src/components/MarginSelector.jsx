import React from "react";

const MarginSelector = ({ margins, onChange }) => {
  const marginOptions = [
    { key: 'top', label: 'Top' },
    { key: 'right', label: 'Right' },
    { key: 'bottom', label: 'Bottom' },
    { key: 'left', label: 'Left' }
  ];

  const handleMarginChange = (key, value) => {
    const newMargins = { ...margins, [key]: Number(value) };
    onChange(newMargins);
  };

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-medium text-white">Margin Settings</h3>
      
      <div className="grid grid-cols-2 gap-4">
        {marginOptions.map(({ key, label }) => (
          <div key={key}>
            <label className="block text-sm font-medium text-white mb-2">
              {label} (mm)
            </label>
            <input
              type="number"
              value={margins?.[key] || 20}
              min={0}
              max={100}
              onChange={(e) => handleMarginChange(key, e.target.value)}
              className="w-full px-3 py-2 bg-[#0D1164]/50 border border-[#640D5F] rounded-lg text-white placeholder-[#F78D60]/60 focus:outline-none focus:ring-2 focus:ring-[#EA2264] focus:border-[#F78D60]"
            />
          </div>
        ))}
      </div>
      
      <div className="pt-4 border-t border-[#640D5F]">
        <button
          onClick={() => onChange({ top: 20, right: 20, bottom: 20, left: 20 })}
          className="text-sm text-white hover:text-[#EA2264] transition-colors"
        >
          Reset to Default (20mm)
        </button>
      </div>
    </div>
  );
};

export default MarginSelector;
