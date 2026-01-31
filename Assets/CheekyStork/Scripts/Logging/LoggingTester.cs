using System;
using UnityEngine;

namespace CheekyStork.Logging
{
    public class LoggingTester : MonoBehaviour
    {
        [SerializeField] 
        private GameObject nullGameObject;

        [SerializeField] 
        private ScriptableObject objectToBeHighlighted;

        private void Awake()
        {
            this.LogInfo("This is an info log with the audio tag", LogTag.Audio);
            this.LogWarning("This is a warning log with the audio tag", LogTag.Audio);
            this.LogError("This is an error log with the audio tag", LogTag.Audio);
            this.LogDebug("This is a debug log with the audio tag", LogTag.Audio);
            this.LogFatal("This is a fatal log with the audio tag", LogTag.Audio);
            
            this.LogInfo("Logging tester is active. Use Keys F1-F9 to test logging functionality", LogTag.Editor);
        }

        private void Start()
        {
            this.LogInfo("!!!!! Logging tester is active. Use Keys F1-F9 to test logging functionality !!!!!", LogTag.Editor);
            this.LogInfo("!!!!! Logging a bunch of different examples !!!!!", LogTag.Editor);
            this.LogInfo($"This is a log message with a tag of 'Misc', being sent from a MonoBehaviour", LogTag.Misc);
            this.LogInfo($"This is a log message with a tag of 'Editor', being sent from a MonoBehaviour", LogTag.Editor);
            this.LogInfo($"This is a log message with a tag of 'Tools', being sent from a MonoBehaviour", LogTag.Tools);
            this.LogInfo($"This is a log message with a tag of 'Sandbox', being sent from a MonoBehaviour", LogTag.Sandbox);
            this.LogInfo($"This is a log message with a tag of 'Core', being sent from a MonoBehaviour", LogTag.Core);
            this.LogInfo($"This is a log message with a tag of 'Flow', being sent from a MonoBehaviour", LogTag.Flow);
            this.LogInfo($"This is a log message with a tag of 'Player', being sent from a MonoBehaviour", LogTag.Player);
            this.LogInfo($"This is a log message with a tag of 'World', being sent from a MonoBehaviour", LogTag.World);
            this.LogInfo($"This is a log message with a tag of 'GameMaster', being sent from a MonoBehaviour", LogTag.GameMaster);
            this.LogInfo($"This is a log message with a tag of 'UI', being sent from a MonoBehaviour", LogTag.UI);
            this.LogInfo($"This is a log message with a tag of 'AI', being sent from a MonoBehaviour", LogTag.AI);
            this.LogInfo($"This is a log message with a tag of 'Audio', being sent from a MonoBehaviour", LogTag.Audio);
            this.LogInfo($"This is a log message with a tag of 'Animation', being sent from a MonoBehaviour", LogTag.Animation);
            this.LogInfo($"This is a log message with a tag of 'Skills', being sent from a MonoBehaviour", LogTag.Skills);
            this.LogInfo($"This is a log message with a tag of 'Items', being sent from a MonoBehaviour", LogTag.Items);
            this.LogInfo("This is a log message being sent from a MonoBehaviour", LogTag.Tools, LogTag.Audio);
            this.LogDebug("This is a debug message being sent from a MonoBehaviour", LogTag.Tools, LogTag.Audio);
            this.LogWarning("This is a warning message being sent from a MonoBehaviour", LogTag.Sandbox, LogTag.Audio);
            this.LogError("This is an error message being sent from a MonoBehaviour", LogTag.Flow);
            this.LogFatal("This is a fatal message being sent from a MonoBehaviour", LogTag.AI, LogTag.GameMaster);
            this.LogInfo("This is a log message with duplicate (INCORRECT) tags, being sent from a MonoBehaviour", LogTag.GameMaster, LogTag.Audio, LogTag.World, LogTag.Audio);
            this.Assert(1 == 2, "This is a false assertion being sent from a MonoBehaviour", LogTag.Audio);
            this.Assert(2 == 2, "This is a true assertion being sent from a MonoBehaviour", LogTag.Audio);
            this.Assert(1 == 2, "This is a false assertion with a highlighted object, being sent from a MonoBehaviour", objectToBeHighlighted, LogTag.Audio);
            this.LogInfo("This is a log message with a highlighted object, being sent from a MonoBehaviour", objectToBeHighlighted, LogTag.World, LogTag.Audio);
            this.LogInfo("This is a log message with an Audio Tag, being sent from a MonoBehaviour", LogTag.Audio);

            Log.LogInfo("This is a log message being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio);
            Log.LogDebug("This is a debug message being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio);
            Log.LogWarning("This is a warning message being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio);
            Log.LogError("This is an error message being sent from a MonoBehaviour", LogTag.Sandbox, LogTag.Skills);
            Log.LogFatal("This is a fatal message being sent from a MonoBehaviour", LogTag.AI, LogTag.Sandbox);
            Log.LogInfo("This is a log message with duplicate (INCORRECT) tags, being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio, LogTag.Sandbox, LogTag.Audio);
            Log.Assert(1 == 2, "This is a false assertion being sent from a MonoBehaviour", LogTag.Audio);
            Log.Assert(2 == 2, "This is a true assertion being sent from a MonoBehaviour", LogTag.Audio);
            Log.Assert(1 == 2, "This is a false assertion with a highlighted object, being sent from a MonoBehaviour", objectToBeHighlighted, LogTag.Audio);
            Log.LogInfo("This is a log message with a highlighted object, being sent from a MonoBehaviour", objectToBeHighlighted, LogTag.Skills, LogTag.Audio);
            Log.LogInfo("This is a log message with an Audio Tag, being sent from a MonoBehaviour", LogTag.Audio);

            this.LogInfo("This is a log message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            this.LogDebug("This is a debug message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            this.LogWarning("This is a warning message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            this.LogError("This is an error message being sent from a MonoBehaviour", this, LogTag.Sandbox, LogTag.Skills);
            this.LogFatal("This is a fatal message being sent from a MonoBehaviour", this, LogTag.AI, LogTag.Sandbox);
            this.LogInfo("This is a log message with duplicate (INCORRECT) tags, being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio, LogTag.Sandbox, LogTag.Audio);
            this.Assert(1 == 2, "This is a false assertion being sent from a MonoBehaviour", this, LogTag.Audio);
            this.Assert(2 == 2, "This is a true assertion being sent from a MonoBehaviour", this, LogTag.Audio);
            this.Assert(1 == 2, "This is a false assertion with a highlighted object, being sent from a MonoBehaviour", this, LogTag.Audio);
            this.LogInfo("This is a log message with a highlighted object, being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            this.LogInfo("This is a log message with an Audio Tag, being sent from a MonoBehaviour", this, LogTag.Audio);

            Log.LogInfo("This is a log message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            Log.LogDebug("This is a debug message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            Log.LogWarning("This is a warning message being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            Log.LogError("This is an error message being sent from a MonoBehaviour", this, LogTag.Sandbox, LogTag.Skills);
            Log.LogFatal("This is a fatal message being sent from a MonoBehaviour", this, LogTag.AI, LogTag.Sandbox);
            Log.LogInfo("This is a log message with duplicate (INCORRECT) tags, being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio, LogTag.Sandbox, LogTag.Audio);
            Log.Assert(1 == 2, "This is a false assertion being sent from a MonoBehaviour", this, LogTag.Audio);
            Log.Assert(2 == 2, "This is a true assertion being sent from a MonoBehaviour", this, LogTag.Audio);
            Log.Assert(1 == 2, "This is a false assertion with a highlighted object, being sent from a MonoBehaviour", this, LogTag.Audio);
            Log.LogInfo("This is a log message with a highlighted object, being sent from a MonoBehaviour", this, LogTag.Skills, LogTag.Audio);
            Log.LogInfo("This is a log message with an Audio Tag, being sent from a MonoBehaviour", this, LogTag.Audio);

            Debug.Log("This is a Debug.Log message being sent from a MonoBehaviour");
            Debug.LogWarning("This is a Debug.LogWarning message being sent from a MonoBehaviour");
            Debug.LogError("This is a Debug.LogError message being sent from a MonoBehaviour");
            Debug.LogException(new Exception("This is a Debug.LogException message being sent from a MonoBehaviour"));
            Debug.Assert(1 == 1, "This is a Debug.Assert message being sent from a MonoBehaviour");

            Debug.Log("This is a Debug.Log message being sent from a MonoBehaviour", this);
            Debug.LogWarning("This is a Debug.LogWarning message being sent from a MonoBehaviour", this);
            Debug.LogError("This is a Debug.LogError message being sent from a MonoBehaviour", this);
            Debug.LogException(new Exception("This is a Debug.LogException message being sent from a MonoBehaviour"), this);
            Debug.Assert(1 == 1, "This is a Debug.Assert message being sent from a MonoBehaviour", this);
            Debug.Assert(1 == 2, "This is a Debug.Assert message being sent from a MonoBehaviour");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                this.LogInfo("Example of this.log message being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Log.LogWarning("Example of Log.LogWarning message being sent from a MonoBehaviour", LogTag.Sandbox, LogTag.Skills);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                this.LogError("Example of Log.LogError message being sent from a MonoBehaviour, with 'this' as a context object", this, LogTag.Skills, LogTag.AI, LogTag.Sandbox);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                this.LogFatal("Example of this.LogFatal message being sent from a MonoBehaviour", LogTag.Skills, LogTag.Audio, LogTag.Sandbox, LogTag.Audio);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                this.Assert(1 == 2, "Example of this.Assert message being sent from a MonoBehaviour", LogTag.GameMaster);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                Debug.Log("Example of Debug.Log message being sent from a MonoBehaviour");
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                MethodG();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                MethodD();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                MethodA();
            }
        }

        private void MethodA()
        {
            MethodB();
        }

        private void MethodB()
        {
            MethodC();
        }

        private void MethodC()
        {
            Debug.Log(nullGameObject.name);
        }

        private void MethodD()
        {
            MethodE();
        }

        private void MethodE()
        {
            MethodF();
        }

        private void MethodF()
        {
            Debug.LogException(new Exception("Example of Debug.LogException message being sent from a MonoBehaviour"));
        }

        private void MethodG()
        {
            MethodH();
        }

        private void MethodH()
        {
            MethodI();
        }

        private void MethodI()
        {
            Debug.LogError("Example of Debug.LogError message being sent from a MonoBehaviour");
        }
    }
}