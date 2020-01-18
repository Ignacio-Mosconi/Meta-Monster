using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GreenNacho.UI
{
    public class AppMenu : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] Button menuButton = default;
        [SerializeField] Button closeButton = default;

        [Header("Animations")]
        [SerializeField] SlideAnimation slidingMenuAnimation = default;
        [SerializeField] SlideAnimation menuButtonAnimation = default;
        [SerializeField] FadeAnimation backgroundBlockerAnimation = default;

        [Header("Finger Slide Properties")]
        [SerializeField] bool enableMenuSlide = true;
        [SerializeField] RectTransform fingerSlideStartBlocker = default;
        [SerializeField, Range(0.1f, 0.2f)] float viewportDragPositionShow = 0.1f;
        [SerializeField, Range(0f, 1f)] float showDeltaPercentage = 0.5f;
        [SerializeField, Range(0f, 1f)] float hideDeltaPercentage = 0.3f;

        AppMenuButton[] appMenuButtons;
        RectTransform slidingMenuRectTransform;
        Image backgroundBlockerImage;
        Vector2 dragStartPosition;
        float dragAcummulator;
        float bgBlockerInterp;
        float menuActivationDelta;
        float menuDeactivationDelta;
        float showHorDragPosition;
        float hideHorDragPosition;
        bool beganValidDrag;
        bool isSlidePlaying;
        bool isMenuActive;

        const float NegligibleDeltaTouchPosition = 5f;
        const float BeginTouchDeltaTouchPosition = 10f;

        void Awake()
        {
            appMenuButtons = GetComponentsInChildren<AppMenuButton>();
            slidingMenuRectTransform = slidingMenuAnimation.GetComponent<RectTransform>();
            backgroundBlockerImage = backgroundBlockerAnimation.GetComponent<Image>();
        }

        void Start()
        {
            showHorDragPosition = Camera.main.ViewportToScreenPoint(new Vector3(viewportDragPositionShow, 0f, 0f)).x;
            hideHorDragPosition = slidingMenuRectTransform.rect.size.x;
            menuActivationDelta = slidingMenuRectTransform.rect.size.x * showDeltaPercentage;
            menuDeactivationDelta = -slidingMenuRectTransform.rect.size.x * hideDeltaPercentage;

            fingerSlideStartBlocker.anchorMin = Vector2.zero;
            fingerSlideStartBlocker.anchorMax = new Vector2(viewportDragPositionShow, 1f);

            menuButton?.onClick.AddListener(ShowMenu);
            closeButton?.onClick.AddListener(HideMenu);
            
            backgroundBlockerAnimation.Deactivate();
            if (!enableMenuSlide)
                enabled = false;
        }

        void Update()
        {
            if (Input.touchCount != 1 || isSlidePlaying)
                return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Moved:

                    if (!beganValidDrag)
                        CheckValidDragStart(touch);
                    else
                    {
                        UpdateSlidingMenuPosition(touch);
                        UpdateBackgroundBlockerAlpha(touch);
                    }
                    break;

                case TouchPhase.Ended:
                    
                    if (beganValidDrag)
                        CheckMenuActivation(touch);
                    else
                        CheckMenuDeactivationOnSimpleTouch(touch);
                    break;
            }
        }

        void CheckValidDragStart(Touch touch)
        {
            if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
            {
                if (!isMenuActive)
                {
                    if (touch.position.x <= showHorDragPosition && 
                        Mathf.Abs(touch.deltaPosition.x) > BeginTouchDeltaTouchPosition)
                    {
                        beganValidDrag = true;
                        dragStartPosition = touch.position;
                        backgroundBlockerAnimation.Activate();
                    }
                }
                else
                {
                    if (touch.position.x <= hideHorDragPosition && 
                        Mathf.Abs(touch.deltaPosition.x) > BeginTouchDeltaTouchPosition)
                    {
                        beganValidDrag = true;
                        dragStartPosition = touch.position;
                    }
                }

                if (beganValidDrag)
                    SetMenuButtonsInteraction(enableInteraction: false);
            }
        }

        void UpdateSlidingMenuPosition(Touch touch)
        {
            dragAcummulator += touch.deltaPosition.x;

            if (Mathf.Abs(dragAcummulator) >= NegligibleDeltaTouchPosition)
            {
                float newHorPos = slidingMenuRectTransform.anchoredPosition.x + dragAcummulator;

                dragAcummulator = 0f;
                newHorPos = Mathf.Clamp(newHorPos, slidingMenuAnimation.SlideInOffscreenPosition.x, slidingMenuAnimation.InitialPosition.x);
                slidingMenuRectTransform.anchoredPosition = new Vector2(newHorPos, slidingMenuRectTransform.anchoredPosition.y);
            }
        }

        void UpdateBackgroundBlockerAlpha(Touch touch)
        {
            float menuDeltaPosition = Mathf.Abs(slidingMenuAnimation.SlideInOffscreenPosition.x - slidingMenuAnimation.InitialPosition.x);
            Color newColor = backgroundBlockerImage.color;

            bgBlockerInterp += touch.deltaPosition.x / menuDeltaPosition;
            newColor.a = Mathf.Lerp(backgroundBlockerAnimation.TargetHideAlpha, backgroundBlockerAnimation.TargetShowAlpha, bgBlockerInterp);
            backgroundBlockerImage.color = newColor;
        }

        void CheckMenuActivation(Touch touch)
        {
            float horTouchDelta = touch.position.x - dragStartPosition.x;
            float lastHorDelta = touch.deltaPosition.x;
            
            beganValidDrag = false;
            dragStartPosition = touch.position;

            bool shouldShowMenu = (!isMenuActive && horTouchDelta > menuActivationDelta && lastHorDelta > 0);
            bool shouldHideMenu = (isMenuActive && horTouchDelta < menuDeactivationDelta && lastHorDelta < 0);

            if (shouldShowMenu || shouldHideMenu)
            {
                if (shouldShowMenu)
                    ShowMenu();
                else
                    HideMenu();
            }
            else
            {
                if (!isMenuActive)
                {
                    bgBlockerInterp = 0f;
                    slidingMenuAnimation.Hide();
                    Tween blockerHideTween = backgroundBlockerAnimation.Hide();
                    blockerHideTween.OnComplete(backgroundBlockerAnimation.Deactivate);
                }
                else
                {
                    bgBlockerInterp = 1f;
                    backgroundBlockerAnimation.Show();
                    Tween slideTween = slidingMenuAnimation.Show();
                    slideTween.OnComplete(() => SetMenuButtonsInteraction(enableInteraction: true));
                }
            }
        }

        void CheckMenuDeactivationOnSimpleTouch(Touch touch)
        {
            if (isMenuActive && touch.position.x > hideHorDragPosition)
                HideMenu();
        }

        void ToggleMenuStatus()
        {
            isSlidePlaying = false;
            isMenuActive = !isMenuActive;
        }

        void ShowMenu()
        {
            isSlidePlaying = true;
            bgBlockerInterp = 1f;
            backgroundBlockerAnimation.Activate();
            backgroundBlockerAnimation.Show();
            Tween slideTween = slidingMenuAnimation.Show();
            slideTween.OnComplete(() => 
            {
                ToggleMenuStatus();
                SetMenuButtonsInteraction(enableInteraction: true);
            });

            OnShow();
        }

        void SetMenuButtonsInteraction(bool enableInteraction)
        {
            foreach (AppMenuButton appMenuButton in appMenuButtons)
                appMenuButton.SetButtonInteraction(enableInteraction);
        }

        protected virtual void OnShow() {}

        public void HideMenu()
        {
            isSlidePlaying = true;
            bgBlockerInterp = 0f;
            Tween fadeTween = backgroundBlockerAnimation.Hide();
            Tween slideTween = slidingMenuAnimation.Hide();
            fadeTween.OnComplete(backgroundBlockerAnimation.Deactivate);
            slideTween.OnComplete(ToggleMenuStatus);
        }

        public void SetUpAnimations()
        {
            slidingMenuAnimation.SetUp();
            menuButtonAnimation?.SetUp();
            backgroundBlockerAnimation.SetUp();
        }

        public void Activate(Sequence sequence = null)
        {
            gameObject.SetActive(true);
            Tween menuButtonTween = menuButtonAnimation?.Show();
            if (sequence != null)
                sequence.Append(menuButtonTween);
        }

        public void Deactivate(Sequence sequence = null)
        {
            Tween menuButtonTween = menuButtonAnimation?.Hide();
            if (sequence != null)
            {
                sequence.Append(menuButtonTween);
                sequence.OnComplete(() => gameObject.SetActive(false));
            }
            else
                gameObject.SetActive(false);
        }

        #region Properties
        
        public bool BeganValidDrag
        {
            get { return beganValidDrag; }
        }

        public bool IsSlidePlaying
        {
            get { return isSlidePlaying; }
        }

        public bool IsMenuActive
        {
            get { return isMenuActive; }
        }
        
        #endregion
    }
}