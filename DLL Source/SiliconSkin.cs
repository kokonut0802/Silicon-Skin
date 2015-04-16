using ColossalFramework.UI;
using UnityEngine;

namespace SiliconSkin
{
    public class SiliconSkin : MonoBehaviour
    {

        private UIMultiStateButton hideUiButton;
        private UILabel hideController;
        private UILabel infoPanelTextColors;

        private bool inTransition = false;
        private bool transitionDirection = false;
        private float currentY = 0.0f;

        private UIComponent tsBar;
        private UIComponent infoPanel;
        private UIComponent infoButton;
        private UIComponent infoViewsContainer;

        private Bindings uiBindings;
        private UIComponent timeBarColorsComponent;

        private Vector3 tsBarOriginalPosition;
        private Vector3 infoPanelOriginalPosition;
        private Vector3 infoButtonOriginalPosition;
        private Vector3 infoViewsContainerOriginalPosition;
        private bool originalPositionsInitialized = false;

        private UILabel populationChange;
        private UILabel moneyChange;
        private UIButton moneyLabel;

        private UITiledSprite myBulldozerBar;
        private UITiledSprite bulldozerBar;

        private bool bulldozerBarInTransition = false;
        private bool bulldozerBarTransitionDirection = false;

        void Awake()
        {
            bulldozerBar = GameObject.Find("BulldozerBar").GetComponent<UITiledSprite>();
            oldBulldozerSpriteName = bulldozerBar.spriteName;

            myBulldozerBar = Instantiate(bulldozerBar).GetComponent<UITiledSprite>();
            myBulldozerBar.transform.parent = FindObjectOfType<UIView>().transform;
            myBulldozerBar.relativePosition = new Vector3(0.0f, -myBulldozerBar.height);
            myBulldozerBar.isVisible = true;

            tsBar = GameObject.Find("TSBar").GetComponent<UIComponent>();
            infoPanel = GameObject.Find("InfoPanel").GetComponent<UIComponent>();

            infoPanelTextColors = infoPanel.AddUIComponent<UILabel>();
            infoPanelTextColors.name = "SiliconTextColors";
            infoPanelTextColors.color = Color.white;
            infoPanelTextColors.disabledColor = Color.white;

            infoButton = infoPanel.Find("InfoMenu");
            infoViewsContainer = infoPanel.Find("InfoViewsContainer");

            uiBindings = FindObjectOfType<UIView>().GetComponent<Bindings>();

            timeBarColorsComponent = GameObject.Find("PanelTime").GetComponent<UIComponent>().AddUIComponent<UILabel>();
            timeBarColorsComponent.name = "TimeBarColors";
            timeBarColorsComponent.isVisible = false;
            timeBarColorsComponent.color = Color.green;
            timeBarColorsComponent.disabledColor = Color.red;

            moneyChange = GameObject.Find("ProjectedIncome").GetComponent<UILabel>();
            moneyChange.textColor = infoPanelTextColors.color;
            
            populationChange = GameObject.Find("ProjectedChange").GetComponent<UILabel>();
            populationChange.textColor = infoPanelTextColors.color;

            moneyLabel = GameObject.Find("IncomePanel").GetComponent<UIButton>();
            moneyLabel.textColor = infoPanelTextColors.color;
            
            CreateHideUiButton();
        }

        void OnDestroy()
        {
            Destroy(myBulldozerBar.gameObject);
            Destroy(hideUiButton.gameObject);
            Destroy(timeBarColorsComponent.gameObject);
        }

        private void CreateHideUiButton()
        {
            hideUiButton = FindObjectOfType<UIView>().AddUIComponent(typeof(UIMultiStateButton)) as UIMultiStateButton;

            hideUiButton.name = "SiliconSkinHideUiButton";
            hideUiButton.size = new Vector2(128.0f, 128.0f);
            hideUiButton.relativePosition = new Vector3(0.0f, -256.0f, 0.0f);
            hideUiButton.text = "CLICK ME";
            hideUiButton.textScale = 1.5f;
            hideUiButton.spritePadding = new RectOffset(0, 0, 0, 0);
            hideUiButton.foregroundSprites.AddState();
            hideUiButton.backgroundSprites.AddState();
            hideUiButton.activeStateIndex = 0;
            hideUiButton.isVisible = false;

            hideController = hideUiButton.AddUIComponent<UILabel>();
            hideController.name = "HideControls";
            hideController.size = new Vector2(192.0f, 120.0f);

            hideUiButton.eventClick += (component, param) =>
            {
                if (!originalPositionsInitialized)
                {
                    tsBarOriginalPosition = tsBar.relativePosition;
                    infoPanelOriginalPosition = infoPanel.relativePosition;
                    infoButtonOriginalPosition = infoButton.relativePosition;
                    infoViewsContainerOriginalPosition = infoViewsContainer.relativePosition;
                    originalPositionsInitialized = true;
                }

                if (inTransition)
                { 
                    return;
                }

                inTransition = true;
                transitionDirection = currentY == 0.0f;
                hideUiButton.isEnabled = false;
            };
        }

        private string oldBulldozerSpriteName = "";

        void Update()
        {
            if (hideUiButton.isVisible == false)
            {
                bulldozerBar.spriteName = oldBulldozerSpriteName;
                return;
            }
            else
            {
                bulldozerBar.spriteName = "";
            }


            if (!bulldozerBarInTransition && bulldozerBar.isVisible && myBulldozerBar.relativePosition.y < 0.0f)
            {
                bulldozerBarInTransition = true;
                bulldozerBarTransitionDirection = true;
            }
            else if (!bulldozerBarInTransition && !bulldozerBar.isVisible && myBulldozerBar.relativePosition.y == 0.0f)
            {
                bulldozerBarInTransition = true;
                bulldozerBarTransitionDirection = false;
            }
            else if (bulldozerBarInTransition)
            {
                if (bulldozerBarTransitionDirection)
                {
                    myBulldozerBar.relativePosition += new Vector3(0.0f, Time.deltaTime * 128.0f, 0.0f);
                    if (myBulldozerBar.relativePosition.y >= 0.0f)
                    {
                        myBulldozerBar.relativePosition = new Vector3(0.0f, 0.0f);
                        bulldozerBarInTransition = false;
                    }
                }
                else
                {
                    myBulldozerBar.relativePosition -= new Vector3(0.0f, Time.deltaTime * 128.0f, 0.0f);
                    if (myBulldozerBar.relativePosition.y <= -myBulldozerBar.height)
                    {
                        myBulldozerBar.relativePosition = new Vector3(0.0f, -myBulldozerBar.height);
                        bulldozerBarInTransition = false;
                    }
                }
            }

            uiBindings.m_PlayingColor = timeBarColorsComponent.color;
            uiBindings.m_PausedColor = timeBarColorsComponent.disabledColor;

            var popChangeText = populationChange.text;
            if (popChangeText.Contains(">") && popChangeText.Contains("<"))
            {
                populationChange.text = popChangeText.Substring(15).Split('<')[0];
                if (populationChange.text[0] == '-')
                {
                    populationChange.textColor = infoPanelTextColors.disabledColor;
                }
                else
                {
                    populationChange.textColor = infoPanelTextColors.color;
                }
            }

            var moneyChangeText = moneyChange.text;
            if (moneyChangeText.Contains(">") && moneyChangeText.Contains("<"))
            {
                moneyChange.text = moneyChangeText.Substring(15).Split('<')[0];
                if (moneyChange.text[0] == '-')
                {
                    moneyChange.textColor = infoPanelTextColors.disabledColor;
                }
                else
                {
                    moneyChange.textColor = infoPanelTextColors.color;
                }
            }

            var moneyText = moneyLabel.text;
            if (!moneyText.Contains("-"))
            {
                moneyLabel.textColor = infoPanelTextColors.color;
            }
            else
            {
                moneyLabel.textColor = infoPanelTextColors.disabledColor;
            }

            if (!inTransition)
            {
                return;
            }

            currentY += (transitionDirection ? -1.0f : 1.0f)*Time.deltaTime*hideController.size.x;
            if (transitionDirection && currentY <= -hideController.size.y)
            {
                currentY = -hideController.size.y;
                inTransition = false;
                hideUiButton.activeStateIndex = 1;
                hideUiButton.isEnabled = true;
            }
            else if (!transitionDirection && currentY >= 0.0f)
            {
                currentY = 0.0f;
                inTransition = false;
                hideUiButton.activeStateIndex = 0;
                hideUiButton.isEnabled = true;
            }

            tsBar.relativePosition = tsBarOriginalPosition + new Vector3(0.0f, -currentY, 0.0f);
            infoPanel.relativePosition = infoPanelOriginalPosition + new Vector3(0.0f, -currentY, 0.0f);
            infoButton.relativePosition = infoButtonOriginalPosition + new Vector3(0.0f, currentY, 0.0f);
            infoViewsContainer.relativePosition = infoViewsContainerOriginalPosition + new Vector3(0.0f, currentY, 0.0f);
        }
    }
}
