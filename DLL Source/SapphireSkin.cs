using ICities;
using UnityEngine;

namespace SiliconSkin
{

    public class Mod : IUserMod
    {

        public string Name
        {
            get { return "Silicon"; }
        }

        public string Description
        {
            get { return "Silicon Skin for Sapphire UI"; }
        }

    }

    public class ModLoad : LoadingExtensionBase
    {

        private GameObject uiObject;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            { 
                uiObject = new GameObject("SiliconSkin");
                uiObject.AddComponent<SiliconSkin>();
            }
        }

        public override void OnLevelUnloading()
        {
            if (uiObject != null)
            {
                GameObject.Destroy(uiObject);
            }
        }

    }
}
