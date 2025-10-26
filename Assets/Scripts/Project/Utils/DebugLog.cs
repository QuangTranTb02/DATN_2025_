using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Project.Utils
{
    /// <summary>
    /// source: https://gist.github.com/Hertzole/d28243075a1074ce7b2ff713fcfe8573
    /// </summary>
    public class DebugRuntimeConsole : MonoBehaviour
    {
        /// <summary>
        /// A struct to hold all the info about a debug message.
        /// </summary>
        private readonly struct DebugMessage
        {
            /// <summary> The condition text. </summary>
            public readonly string Condition;
            /// <summary> The stack trace. </summary>
            public readonly string StackTrace;
            /// <summary> The type of the log. </summary>
            public readonly LogType Type;

            public DebugMessage(string condition, string stackTrace, LogType type)
            {
                Condition = condition;
                StackTrace = stackTrace;
                Type = type;
            }
        }

        [SerializeField]
        [Tooltip("Determines if the console should be enabled.")]
        private bool m_EnableConsole = false;
        [SerializeField]
        [Tooltip("The key to press to toggle the visibility of the console.")]
        private KeyCode m_ToggleKey = KeyCode.F8;
        [SerializeField]
        [Tooltip("Determines if timestamps should be added to the log message.")]
        private bool m_ShowTimestamps = true;
        [SerializeField]
        [Tooltip("Determines if the game object should be kept through scenes.")]
        private bool m_DontDestroyOnLoad = true;
        [SerializeField]
        [Tooltip("Determines if the logs can be exported into a text file.")]
        private bool m_CanExportLogs = true;
        [SerializeField]
        [Tooltip("An optional IMGUI skin.")]
        private GUISkin m_GUISkin;

        [Header("Color Settings")]
        [SerializeField]
        [Tooltip("The color for the basic log message.")]
        private Color m_LogColor = Color.white;
        [SerializeField]
        [Tooltip("The color for warning messages.")]
        private Color m_WarningColor = Color.yellow;
        [SerializeField]
        [Tooltip("The color for error messages.")]
        private Color m_ErrorColor = Color.red;
        [SerializeField]
        [Tooltip("The color for exception messages.")]
        private Color m_ExceptionColor = Color.red;
        [SerializeField]
        [Tooltip("The color for assert messages.")]
        private Color m_AssertColor = Color.red;

        // The entry to be expanded. -1 means no entry.
        private int m_ExpandedEntry = -1;

        // Determines if the console should be shown.
        private bool m_Show;

        // The Vector2 for the scroll bar.
        private Vector2 m_Scroll;

        // All the messages.
        private readonly List<DebugMessage> m_Messages = new();
        
        // string builder for exporting logs
        private StringBuilder m_sb;

        void Start()
        {
            // If Dont Destroy On Load is marked, mark the game object as such.
            if (m_DontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);

                // If it can find another debug console, destroy the object current game object.
                DebugRuntimeConsole otherConsole = FindObjectOfType<DebugRuntimeConsole>();
                if (otherConsole != null && otherConsole.transform != this.transform)
                    Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // Subscribe to the log message received event.
            Application.logMessageReceived += OnGetLogMessage;
        }

        private void OnDisable()
        {
            // Unsubscribe to the log message received event.
            Application.logMessageReceived -= OnGetLogMessage;
        }

        private void OnGetLogMessage(string condition, string stackTrace, LogType type)
        {
            // If Show Timestamps is checked, add a timestamp with the current hour, minute and second.
            if (m_ShowTimestamps)
            {
                string originalCondition = condition;
                condition = $"[ {System.DateTime.Now.Hour} : {System.DateTime.Now.Minute} : {System.DateTime.Now.Second} ] {originalCondition}";
            }

            // Trim the condition and stack trace.
            condition = condition.Trim();
            stackTrace = stackTrace.Trim();

            // Add the message.
            m_Messages.Add(new DebugMessage(condition, stackTrace, type));
        }

        private void Update()
        {
            // Show when toggle key is pressed and console is enabled.
            if (Input.GetKeyDown(m_ToggleKey) && m_EnableConsole)
            {
                m_Show = !m_Show;
            }
        }

        void OnGUI()
        {
            // Only run if show and enable console is enabled.
            if (m_Show && m_EnableConsole)
            {
                // If there's a skin, apply it.
                if (m_GUISkin != null)
                    GUI.skin = m_GUISkin;

                // Begin an area.
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, 300), GUI.skin.box);
                // Begin the scroll area.
                m_Scroll = GUILayout.BeginScrollView(m_Scroll);

                // Loop through every message.
                for (int i = 0; i < m_Messages.Count; i++)
                {
                    // Create a new label style and base it on the original label.
                    GUIStyle labelStyle = GUI.skin.label;
                    // Create a entry color field.
                    Color entryColor = Color.magenta;
                    // Get the entry color based on the message type.
                    switch (m_Messages[i].Type)
                    {
                        case LogType.Error:
                            entryColor = m_ErrorColor;
                            break;
                        case LogType.Assert:
                            entryColor = m_AssertColor;
                            break;
                        case LogType.Warning:
                            entryColor = m_WarningColor;
                            break;
                        case LogType.Log:
                            entryColor = m_LogColor;
                            break;
                        case LogType.Exception:
                            entryColor = m_ExceptionColor;
                            break;
                    }
                    // Set the text color to the entry color.
                    labelStyle.normal.textColor = entryColor;

                    // Create a button that looks like a label.
                    if (GUILayout.Button(m_Messages[i].Condition, labelStyle))
                    {
                        // When clicked, if the selected entry is the same as this message, close the entry.
                        // Else set the selected entry to thie message entry.
                        if (m_ExpandedEntry == i)
                            m_ExpandedEntry = -1;
                        else
                            m_ExpandedEntry = i;
                    }

                    // If the expanded entry is this message, show the stack trace.
                    if (m_ExpandedEntry == i)
                    {
                        // Create a new box style based on the GUI skin box.
                        GUIStyle boxStyle = GUI.skin.box;
                        // Set the text alignment.
                        boxStyle.alignment = TextAnchor.UpperLeft;
                        // Set the text color to the same color as the entry color.
                        boxStyle.normal.textColor = entryColor;
                        // Show it as a box.
                        GUILayout.Box(m_Messages[i].StackTrace, boxStyle);
                    }
                }

                // End the scroll view.
                GUILayout.EndScrollView();

                // If Can Export Logs is checked, show the export button.
                if (m_CanExportLogs)
                {
                    // When click, do the export logs function.
                    if (GUILayout.Button("Export Logs"))
                    {
                        ExportLogs();
                    }
                }

                // And lastly end the area.
                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// Exports all logs to Application.dataPath/logs
        /// </summary>
        public void ExportLogs()
        {
            // If Can Export Logs is unchecked, stop here.
            if (!m_CanExportLogs)
                return;

            m_sb ??= new StringBuilder();
            // Loop through all the messages.
            for (int i = 0; i < m_Messages.Count; i++)
            {
                // Add the message type to the line.
                switch (m_Messages[i].Type)
                {
                    case LogType.Error:
                        m_sb.Append("[ERROR] ");
                        break;
                    case LogType.Assert:
                        m_sb.Append("[ASSERT] ");
                        break;
                    case LogType.Warning:
                        m_sb.Append("[WARNING] ");
                        break;
                    case LogType.Log:
                        m_sb.Append("[LOG] ");
                        break;
                    case LogType.Exception:
                        m_sb.Append("[EXCEPTION] ");
                        break;
                    default:
                        m_sb.Append("[UNKNOWN TYPE] ");
                        break;
                }

                // Append the condition.
                m_sb.Append(m_Messages[i].Condition);
                // Start a new line.
                m_sb.AppendLine();
                // Add the stack trace.
                m_sb.AppendLine(m_Messages[i].StackTrace);
                // Add a new empty line.
                m_sb.AppendLine();
            }

            // Make sure the output directory exists.
            string filePath = Path.Combine(Application.dataPath, "output_logs");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            // Create the file name and then the file.
            System.DateTime now = System.DateTime.Now;
            string fileName = string.Format("{0}-{1}-{2} {3}-{4}-{5}.txt", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            File.WriteAllText(Path.Combine(filePath, fileName), m_sb.ToString());
            m_sb.Clear();
        }

        #if UNITY_EDITOR
        void OnValidate(){
            if (m_CanExportLogs){
                m_sb ??= new StringBuilder();
            }
            else{
                m_sb = null;
            }

        }
        #endif
    }
}