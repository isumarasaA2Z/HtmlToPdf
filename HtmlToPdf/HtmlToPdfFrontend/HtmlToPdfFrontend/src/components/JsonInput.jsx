import { useState } from 'react'

const JsonInput = ({ value, onChange }) => {
  const [error, setError] = useState('')
  const [isValid, setIsValid] = useState(true)

  const handleInputChange = (e) => {
    const newValue = e.target.value
    onChange(newValue)
    
    // Validate JSON
    if (newValue.trim()) {
      try {
        JSON.parse(newValue)
        setError('')
        setIsValid(true)
      } catch (err) {
        setError('Invalid JSON format')
        setIsValid(false)
      }
    } else {
      setError('')
      setIsValid(true)
    }
  }

  const sampleJson = {
    "reportData": {
      "texts": [
        {
          "name": "orderNO",
          "value": "PO-2024-001"
        },
        {
          "name": "orderno1", 
          "value": "REF-12345"
        }
      ],
      "tables": [
        {
          "name": "Order",
          "headers": ["Order No", "Line No", "Part No", "Quantity"],
          "rows": [
            {
              "columns": ["PO-001", "1", "PART-A123", "100"]
            },
            {
              "columns": ["PO-001", "2", "PART-B456", "50"]
            }
          ],
          "tableMetaData": "style='width: 100%; border-collapse: collapse;'",
          "headerRowMetaData": "style='background-color: #f4f4f4;'",
          "headerCellMetaData": "style='border: 1px solid #ddd; padding: 8px; text-align: left;'",
          "rowMetaData": "",
          "cellMetaData": "style='border: 1px solid #ddd; padding: 8px;'"
        }
      ]
    }
  }

  const insertSample = () => {
    const jsonString = JSON.stringify(sampleJson, null, 2)
    onChange(jsonString)
    setError('')
    setIsValid(true)
  }

  return (
    <div className="bg-white/10 backdrop-blur-md rounded-xl border border-white/20 p-6 h-full flex flex-col">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold text-white">JSON Input</h2>
        <button
          onClick={insertSample}
          className="px-4 py-2 bg-[#F78D60] hover:bg-[#F78D60]/80 text-white rounded-lg font-medium transition-colors"
        >
          Insert Sample
        </button>
      </div>
      
      <div className="flex-1 flex flex-col">
        <textarea
          value={value}
          onChange={handleInputChange}
          placeholder="Paste your JSON data here..."
          className={`flex-1 w-full p-4 bg-[#0D1164]/20 border rounded-lg text-white placeholder-gray-300 font-mono text-sm resize-none focus:outline-none focus:ring-2 transition-colors ${
            isValid 
              ? 'border-gray-300/30 focus:ring-[#F78D60]/50' 
              : 'border-red-500 focus:ring-red-500/50'
          }`}
        />
        
        {error && (
          <div className="mt-3 p-3 bg-red-500/20 border border-red-500/30 rounded-lg">
            <p className="text-red-300 text-sm">{error}</p>
          </div>
        )}
        
        {isValid && value && (
          <div className="mt-3 p-3 bg-green-500/20 border border-green-500/30 rounded-lg">
            <p className="text-green-300 text-sm">✓ Valid JSON format</p>
          </div>
        )}
      </div>
      
      <div className="mt-4 text-xs text-white">
        <p className="mb-2">Expected format:</p>
        <ul className="list-disc list-inside space-y-1 text-white">
          <li>texts: Array with name/value pairs for #placeholders#</li>
          <li>tables: Array with name, headers, and rows</li>
          <li>pageSetup: Page configuration and styling</li>
        </ul>
      </div>
    </div>
  )
}

export default JsonInput
