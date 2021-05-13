using ControlsLibrary.Model;

namespace ControlsLibrary.Controls.TestPanel
{
    /// <summary>
    /// Data to test serialization/deserialization
    /// </summary>
    public class TestSerializationData
    {
        /// <summary>
        /// Input string
        /// </summary>
        public string TestString { get; set; }

        /// <summary>
        /// Should automaton reject string or not
        /// </summary>
        public bool ShouldReject { get; set; }

        /// <summary>
        /// Actual test result
        /// </summary>
        public ResultEnum Result { get; set; }
    }
}
