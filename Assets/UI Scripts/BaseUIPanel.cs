using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUIPanel : MonoBehaviour, IUIPanel
{
    [Header("Panel Info")]
    public string PanelName { get => _panelName; set => _panelName = value; }

    public bool IsActive => gameObject.activeSelf;
    [SerializeField] private string _panelName;

    [SerializeField] bool resetPanel = false;

    Animator animator;

    int hideHash = Animator.StringToHash("Hide");
    int showHash = Animator.StringToHash("Show");
    int idleHash = Animator.StringToHash("Idle");


    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Start()
    { }

    public void Show()
    {
        if (resetPanel)
        {
            OnReset();
        }

    }

    public virtual void OnReset()
    {

    }

    public void Hide()
    {

    }

    public IEnumerator HideCoroutine()
    {
        if (animator == null) yield break;

        bool animation = animator.HasState(0, hideHash);

        if (animation)
        {
            animator?.Play(hideHash);
            yield return WaitForAnimation("Hide");
        }

        else yield break;
    }

    public IEnumerator ShowCoroutine(bool instant)
    {
        if (animator == null) yield break;

        bool animation = animator.HasState(0, instant ? idleHash : showHash);

        if (animation)
        {
            animator?.Play(instant ? idleHash : showHash);
            yield return WaitForAnimation(instant ? "Idle" : "Show");
        }
        else yield break;
    }

    protected IEnumerator WaitForAnimation(string stateName)
    {
        var clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (clipInfo.IsName(stateName) && clipInfo.normalizedTime < 1f)
        {
            yield return null;
            clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}