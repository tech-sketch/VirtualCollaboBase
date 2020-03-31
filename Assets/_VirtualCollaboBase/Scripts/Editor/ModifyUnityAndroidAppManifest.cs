#if UNITY_ANDROID
using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;

public class ModifyUnityAndroidAppManifest : IPostGenerateGradleAndroidProject
{
    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        var isVr = PlayerSettings.GetVirtualRealitySupported(BuildTargetGroup.Android);
        var vrSdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Android);
        var hasOculus = Array.Exists(vrSdks, s => s == OVRManager.OCULUS_UNITY_NAME_STR);

        if (isVr && hasOculus)
        {
            if (OVRDeviceSelector.isTargetDeviceQuest)
            {
                var androidManifest = new AndroidManifest(GetManifestPath(basePath));
                androidManifest.EnableQuestApp();
                androidManifest.Save();
            }
        }

    }

    public int callbackOrder => 10;

    private string _manifestFilePath;

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }

        return _manifestFilePath;
    }
}


internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    protected XmlNamespaceManager nsMgr;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
    public readonly string ToolsXmlNamespace = "http://schemas.android.com/tools";

    public AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }

        nsMgr = new XmlNamespaceManager(NameTable);
        nsMgr.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }

        return path;
    }
}

internal class AndroidManifest : AndroidXmlDocument
{
    private readonly XmlElement ApplicationElement;

    public AndroidManifest(string path) : base(path)
    {
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value)
    {
        var attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    private XmlAttribute CreateToolsAttribute(string key, string value)
    {
        var attr = CreateAttribute("tools", key, ToolsXmlNamespace);
        attr.Value = value;
        return attr;
    }

    public void EnableQuestApp()
    {
        const string headTrackingName = "android.hardware.vr.headtracking";
        var headTrackingData = SelectNodes($"/manifest/uses-feature[@android:name='{headTrackingName}']", nsMgr);
        if (headTrackingData != null && headTrackingData.Count > 0)
        {
            return;
        }

        var manifest = SelectSingleNode("/manifest");
        if (manifest == null)
        {
            return;
        }

        // /manifest に <uses-feature android:name="android.hardware.vr.headtracking" android:required="true" android:version="1" tools:node="merge" /> を追加する
        var newUsesFeature = CreateElement("uses-feature");
        newUsesFeature.Attributes.Append(CreateAndroidAttribute("name", headTrackingName));
        newUsesFeature.Attributes.Append(CreateAndroidAttribute("required", "true"));
        newUsesFeature.Attributes.Append(CreateAndroidAttribute("version", "1"));
        newUsesFeature.Attributes.Append(CreateToolsAttribute("node", "merge"));

        manifest.AppendChild(newUsesFeature);
    }
}
#endif
