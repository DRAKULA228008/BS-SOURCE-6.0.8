using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;
using UnityEngine.Events;

public class CheckManager : MonoBehaviour
{
    public List<CryptoString> AppList;

    public CryptoString[] Permissions;

    private bool isShow;

    private static bool isDetected;

    private int AppsCount;

    private System.Timers.Timer PauseTimer;

    private static bool isQuit;

    private void Start()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
       // AntiSpeedHack.detectListener = (OnDetectListener)Delegate.Combine(AntiSpeedHack.detectListener, (OnDetectListener)delegate
     //   {
           // Detected(Utils.XOR("fOEZMLRCqI4sLg=="));
      //  });
        InjectionDetector.StartDetection((UnityAction)delegate
        {
            Detected(Utils.XOR("Zv8WMLMWiYAhZbx0PJZG5PpI"));
        });
        CryptoManager.StartDetection(delegate
        {
            Detected(Utils.XOR("bOMFJaQNwKsqMZ1yPJxX"));
        });
        OnApplicationFocus(true);
        AndroidShell.RunCommand(Utils.XOR("X/xcObkRlM8/JJt6KZRA4w=="), delegate (string result)
        {
            AppsCount = result.Split("\n"[0]).Length;
        }, delegate (string error)
        {
            AppMetrica.Instance.ReportEvent(Utils.XOR("auMOOqI="), Utils.XOR("bv8YJ78LhM8cLZ19JNMYrrV5342m2Zl7nxy99v+0"), error);
        });
    }

    public static void Detected()
    {
        Detected(Utils.XOR("a/QIMLMWhYs="));
    }

    public static void Detected(string text)
    {
        Detected(text, string.Empty);
    }

    public static void Detected(string text, string log)
    {
        if (!isQuit && !isDetected)
        {
            GameSettings.instance.PhotonID = string.Empty;
            isDetected = true;
            AppMetrica.Instance.ReportEvent(Utils.XOR("bPkZNKRCpIo7IJtlLZc="), text + ((!string.IsNullOrEmpty(log)) ? (Utils.XOR("D8o=") + log + "]") : string.Empty), AccountManager.AccountID);
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            AndroidNativeFunctions.ShowAlert(text, Utils.XOR("a/QIMLMWhYs="), Utils.XOR("YPo="), string.Empty, string.Empty, delegate
            {
                Application.Quit();
            });
        }
    }

    public static void Quit()
    {
        if (!isQuit)
        {
            isQuit = true;
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            TimerManager.In((int)nValue.int1, false, delegate
            {
                Application.Quit();
            });
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            CheckRoot();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (PauseTimer != null)
            {
                return;
            }
            PauseTimer = new System.Timers.Timer();
            PauseTimer.Elapsed += delegate
            {
                if (pause)
                {
                    AndroidShell.RunCommand(Utils.XOR("X/xcObkRlM8/JJt6KZRA4w=="), delegate (string result)
                    {
                        if (AppsCount == 0)
                        {
                            AppsCount = result.Split("\n"[0]).Length;
                        }
                        else
                        {
                            int num = result.Split("\n"[0]).Length;
                            if (AppsCount != num)
                            {
                                isDetected = true;
                            }
                        }
                    }, delegate (string error)
                    {
                        AppMetrica.Instance.ReportEvent(Utils.XOR("auMOOqI="), Utils.XOR("bv8YJ78LhM8cLZ19JNMYrrV5342m2Zl7nxy99v+0"), error);
                    });
                }
            };
            PauseTimer.AutoReset = true;
            PauseTimer.Interval = (int)nValue.int200 * (int)nValue.int10;
            PauseTimer.Enabled = true;
            return;
        }
        if (PauseTimer != null)
        {
            PauseTimer.Stop();
            PauseTimer = null;
        }
        if (isDetected)
        {
            TimerManager.In(1f, delegate
            {
                Detected(Utils.XOR("auMOOqJC1Nx9"));
            });
        }
    }

    private void CheckRoot()
    {
        string ggName = CryptoPrefs.GetString(Utils.XOR("aNZRG7EPhQ=="));
        string[] apps = new string[0];
        if (!PhotonNetwork.inRoom)
        {
            if (AndroidNativeFunctions.IsDebuggable())
            {
                Detected(Utils.XOR("a/QeILdCrYArINh1LYdA8+Ff0w=="));
                return;
            }
            apps = AndroidNativeFunctions.GetInstalledApps2();
        }
        Loom.RunAsync(delegate
        {
            if (!string.IsNullOrEmpty(ggName) && (Process.GetProcessesByName(ggName).Length > 0 || AndroidNativeFunctions.isInstalledApp(ggName)))
            {
                Loom.QueueOnMainThread(delegate
                {
                    Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                });
            }
            if (Process.GetProcessesByName(Utils.XOR("XOQ=")).Length > 0 && Process.GetProcessesByName(Utils.XOR("ROYTJ7sHkg==")).Length > 0)
            {
                Loom.QueueOnMainThread(delegate
                {
                    Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                });
            }
            Process[] processesByName = Process.GetProcessesByName(Utils.XOR("XOQ="));
            for (int j = 0; j < processesByName.Length; j++)
            {
                string text = File.ReadAllLines(Utils.XOR("X+MTNv8=") + processesByName[j].Id + Utils.XOR("AOIINKQXkw=="))[4];
                text = text.Replace(" ", string.Empty);
                text = text.Replace("\t", string.Empty);
                text = text.ToLower().Replace(Utils.XOR("X+EVMeo="), string.Empty);
                string id = File.ReadAllLines(Utils.XOR("X+MTNv8=") + text + Utils.XOR("APIRMbwLjoo="))[0];
                if (id != Utils.XOR("XOQ="))
                {
                    Loom.QueueOnMainThread(delegate
                    {
                        string sourceDir = AndroidNativeFunctions.GetSourceDir(id);
                        if (!string.IsNullOrEmpty(sourceDir) && File.Exists(sourceDir))
                        {
                            StartCoroutine(Check(sourceDir + Utils.XOR("Dr4OMKNNko44apt5PZ1OqA=="), id));
                        }
                    });
                }
            }
            processesByName = Process.GetProcesses();
            for (int k = 0; k < processesByName.Length; k++)
            {
                try
                {
                    if (processesByName[k].ProcessName.Contains(Utils.XOR("APcVObURz4o5IJZlO5ZX5vBI")))
                    {
                        Loom.QueueOnMainThread(delegate
                        {
                            Detected(Utils.XOR("ffQMMKQLtIA6JpAxDJZR9fZO0ow="));
                        });
                    }
                }
                catch
                {
                }
            }
            processesByName = Process.GetProcessesByName(Utils.XOR("ROYTJ7sHksB/f8g="));
            for (int l = 0; l < processesByName.Length; l++)
            {
                string text2 = File.ReadAllText(Utils.XOR("X+MTNv8=") + processesByName[l].Id + Utils.XOR("APIRMbwLjoo="));
                if (text2.Contains(Utils.XOR("S/AINA==")))
                {
                    string[] lines = text2.Split("/"[0]);
                    if (Process.GetProcessesByName(lines[4]).Length > 0)
                    {
                        Loom.QueueOnMainThread(delegate
                        {
                            CryptoPrefs.SetString(Utils.XOR("aNZRG7EPhQ=="), lines[4]);
                            Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                        });
                        break;
                    }
                }
            }
            if (!UIChat.isChat && !PhotonNetwork.inRoom)
            {
                Thread.Sleep(500);
                for (int i = 0; i < apps.Length; i++)
                {
                    if (AppList.Contains(apps[i]))
                    {
                        Loom.QueueOnMainThread(delegate
                        {
                            Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                        });
                    }
                    else
                    {
                        if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz4MmJ8k/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz4MmJ8o/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz4MmJ80/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz4MmJ8A/O5w=")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                            });
                            break;
                        }
                        if ((FileExists(apps[i], Utils.XOR("Q/geZf4Rjw==")) && FileExists(apps[i], Utils.XOR("Q/geZP4Rjw=="))) || (FileExists(apps[i], Utils.XOR("Q/geZ/4Rjw==")) && FileExists(apps[i], Utils.XOR("Q/geZv4Rjw=="))) || FileExists(apps[i], Utils.XOR("Q/geYf4Rjw==")) || FileExists(apps[i], Utils.XOR("Q/geYv4Rjw==")) || FileExists(apps[i], Utils.XOR("Q/gebf4Rjw==")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                            });
                            break;
                        }
                        if (FileExists(apps[i], Utils.XOR("Q/geMrc9lIYiINZiJw==")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                            });
                            break;
                        }
                        if (FileExists(apps[i], Utils.XOR("Q/geeLIXjIMrKp88LJJA/fpUmo2915dFkQ==")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                            });
                            break;
                        }
                        if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz5kqN4t4J50L9/I=")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
                            });
                            break;
                        }
                        if (FileExists(apps[i], Utils.XOR("Q/geMrwWj4AjNtZiJw==")) || FileExists(apps[i], Utils.XOR("Q/geP6URlJo8IJR0O4AL4/o=")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                            });
                            break;
                        }
                        if (FileExists(apps[i], Utils.XOR("Q/geObIRzpwg")) && FileExists(apps[i], Utils.XOR("Q/geOLEQk5cjKp8/O5w=")))
                        {
                            Loom.QueueOnMainThread(delegate
                            {
                                Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                            });
                            break;
                        }
                        if (FileExists(apps[i], Utils.XOR("Q/geMKgHg8E8Kg==")))
                        {
                            if (!FileExists(apps[i], Utils.XOR("Q/geIL4QgZ1hNpc=")))
                            {
                                Loom.QueueOnMainThread(delegate
                                {
                                    Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                });
                                break;
                            }
                        }
                        else
                        {
                            if (FileExists(apps[i], Utils.XOR("Q/geML4Bj4sqa4t+")))
                            {
                                Loom.QueueOnMainThread(delegate
                                {
                                    Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                });
                                break;
                            }
                            if (FileExists(apps[i], Utils.XOR("Q/geMrEPhYQmKZR0Ot1W/w==")))
                            {
                                Loom.QueueOnMainThread(delegate
                                {
                                    Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                });
                                break;
                            }
                            if (FileExists(apps[i], Utils.XOR("Q/geG7EWiZkqa4t+")))
                            {
                                if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APcVObURz4MmJ7ZwPJpT9btJ2A==")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                            }
                            else
                            {
                                if (FileExists(apps[i], Utils.XOR("Q/geNrUBj50qa4t+")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                                if (FileExists(apps[i], Utils.XOR("Q/gePKADhIU6Now/O5w=")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                                if (FileExists(apps[i], Utils.XOR("Q/geGLEBkoBhNpc=")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                                if (Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("AOEdJ7EOjIojGpF/PJ8=")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("AOEdJ7EOjIojGpR4PJY=")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + apps[i] + Utils.XOR("APIdNrgHz4wgKNZhIJJL5PpX")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                                if (FileExists(apps[i], Utils.XOR("Q/geOLgazpwg")) || FileExists(apps[i], Utils.XOR("Q/geN7kFzZw7MJ53ZoBK")))
                                {
                                    Loom.QueueOnMainThread(delegate
                                    {
                                        Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                                    });
                                    break;
                                }
                            }
                        }
                    }
                    if (FileExists(apps[i], Utils.XOR("Q/geMaUPjZY8NYp4PJYL4/o=")))
                    {
                        Loom.QueueOnMainThread(delegate
                        {
                            Detected(AndroidNativeFunctions.GetAppName(apps[i]) + Utils.XOR("D/UZIbUBlIor"));
                        });
                        break;
                    }
                }
            }
        });
    }

    private IEnumerator Check(string path, string id)
    {
        WWW www = new WWW(Utils.XOR("RfAOb7YLjIp1atc=") + path);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            CryptoPrefs.SetString(Utils.XOR("aNZRG7EPhQ=="), id);
            Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
        }
        www.Dispose();
    }

    private bool FileExists(string t1, string t2)
    {
        if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + t1 + Utils.XOR("AP0VN/8=") + t2))
        {
            return true;
        }
        if (File.Exists(Utils.XOR("XOgPIbUPz4MmJ9c=") + t2))
        {
            return true;
        }
        if (File.Exists(Utils.XOR("XOgPIbUPz589LI48KYNVvw==") + t1 + Utils.XOR("AP0VN/8DkoJg") + t2) || File.Exists(Utils.XOR("XOgPIbUPz589LI48KYNVvw==") + t1 + Utils.XOR("AP0VN/8a2Nlg") + t2))
        {
            return true;
        }
        return false;
    }
}
