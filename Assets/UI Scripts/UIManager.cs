using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hudLayer;

    [SerializeField] private List<BaseUIPanel> screens = new List<BaseUIPanel>();

    [SerializeField] private List<BaseUIPanel> overlays = new List<BaseUIPanel>();

    private BaseUIPanel currentScreen;
    private readonly List<BaseUIPanel> activeOverlays = new List<BaseUIPanel>();

    private bool screenTransition;

    public void ShowScreen(string screenName)
    {
        AudioManager.Instance.Play("ButtonClic");

        BaseUIPanel panel = screens.Find(i => i.PanelName == screenName);

        if (panel == null)
        {
            Debug.LogError($"Screen {screenName} not found!");
            return;
        }
        if (panel == currentScreen)
        {
            Debug.LogWarning($"Screen {screenName} is already active!");
            return;
        }

        if (!screenTransition)
        {
            StartCoroutine(ShowScreenRoutine(panel));
        }
    }

    private IEnumerator ShowScreenRoutine(BaseUIPanel nextPanel)
    {
        screenTransition = true;
        if (currentScreen != null)
        {
            yield return currentScreen.HideCoroutine();
            currentScreen.gameObject.SetActive(false);
        }

        currentScreen = nextPanel;
        currentScreen.gameObject.SetActive(true);
        currentScreen.Show();

        yield return currentScreen.ShowCoroutine(false);

        screenTransition = false;
    }

    public void HideScreen()
    {
        AudioManager.Instance.Play("ButtonClic");

        if (screenTransition) return;

        StartCoroutine(HideScreenRoutine());
    }

    private IEnumerator HideScreenRoutine()
    {
        if (currentScreen == null) yield break;

        screenTransition = true;

        BaseUIPanel screenToHide = currentScreen;
        currentScreen = null;

        yield return currentScreen.HideCoroutine();
        currentScreen.gameObject.SetActive(false);
        screenTransition = false;
    }

    public void OpenOverlay(string overlayName)
    {
        AudioManager.Instance.Play("ButtonClic");

        BaseUIPanel panel = overlays.Find(i => i.PanelName == overlayName);

        if (panel == null)
        {
            Debug.LogError($"Overlay {overlayName} not found!");
            return;
        }

        if (activeOverlays.Contains(panel)) return;
        activeOverlays.Add(panel);
        panel.gameObject.SetActive(true);
        panel.Show();

        StartCoroutine(panel.ShowCoroutine(false));
    }

    public void CloseOverlay(string overlayName)
    {
        AudioManager.Instance.Play("ButtonClic");

        BaseUIPanel panel = activeOverlays.Find(i => i.PanelName == overlayName);

        if (panel == null)
        {
            Debug.LogError($"Overlay {overlayName} not found!");
            return;
        }

        if (panel != null)
        {
            StartCoroutine(CloseOverlayRoutine(panel));
        }
    }

    public IEnumerator CloseOverlayRoutine(BaseUIPanel panel)
    {

        activeOverlays.Remove(panel);

        yield return panel.HideCoroutine();

        panel.gameObject.SetActive(false);
    }

    public void CloseAllOverlays()
    {
        AudioManager.Instance.Play("ButtonClic");
        foreach (BaseUIPanel panel in activeOverlays.ToArray())
        {
            if (panel)
            {
                StartCoroutine(CloseOverlayRoutine(panel));
            }
        }
    }

    public void SetHUDOverlays(bool visible)
    {
        if (hudLayer != null)
        {
            hudLayer.SetActive(visible);
        }
    }
}