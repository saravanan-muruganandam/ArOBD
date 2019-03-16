using System;

using System.Text.RegularExpressions;


namespace obd2NET
{
    /// <summary>
    /// Represents the response received from the controller unit.
    /// Answers are supplied with the following syntax:
    /// <c>010D\r\r41 0D 4B\r></c>
    /// <c>010D</c> being the requested PID and <c>41 0D 4B</c> being the value that the controller unit supplied. 
    /// The <c>></c> character is the stop byte for signaling that the data is complete
    /// </summary>
    public class ControllerResponse
    {
        public string Raw { get; set; }
        public ObdAdapter.PID RequestedPID { get; set; }
        public ObdAdapter.Mode RequestedMode { get; set; }

        public Byte[] Value
        {
            get
            {
                if(RequestedPID != ObdAdapter.PID.Unknown && RequestedMode != ObdAdapter.Mode.Unknown)
                {
                    Match matchedPattern = Regex.Match(Raw, @"\n([0-9a-fA-F ]{5})([0-9a-fA-F ]+)\r\n>");
                    return (matchedPattern.Groups.Count > 2) ? matchedPattern.Groups[2].Value.Replace(" ", "").ToByteArray() : Raw.ToByteArray();
                }
                else if (RequestedPID == ObdAdapter.PID.Unknown)
                {
                    Match matchedPattern = Regex.Match(Raw, @"\n([0-9a-fA-F]{2})([0-9a-fA-F ]+)\r\n>");
                    return (matchedPattern.Groups.Count > 2) ? matchedPattern.Groups[2].Value.Replace(" ", "").ToByteArray() : Raw.ToByteArray();
                }
                else
                {
                    Match matchedPattern = Regex.Match(Raw, @"\n([0-9a-fA-F ]+)\r\n>");
                    return (matchedPattern.Groups.Count > 1) ? matchedPattern.Groups[1].Value.Replace(" ", "").ToByteArray() : Raw.ToByteArray();
                }
            }
        }

        public bool HasValidData()
        {
            return Raw.Contains("NO DATA");
        }

        public ControllerResponse(string raw, ObdAdapter.Mode requestedMode = ObdAdapter.Mode.Unknown, ObdAdapter.PID requestedPID = ObdAdapter.PID.Unknown)
        {
            Raw = raw;
            RequestedPID = requestedPID;
            RequestedMode = requestedMode;
        }
    }
}
