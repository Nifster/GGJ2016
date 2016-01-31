using UnityEngine;
using System.Collections;
using UnityEngine;

public class BlackFilter : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_blackFilter = null;
    private GameObject blackFilter;

    private float alpha = 1f;
    private float alphaSpeed = 1.2f;

    private float fadedAlpha = 1f;

    private bool fadingIn;
    
    private float textShowDelay = -1;

    private float lastFrameTime;

    public bool animating { get; private set; }
    private float animationStartRealTime;

    private float textTime_1 = 3f;
    private float textTime_2 = 2.5f;
    private float textTime_3 = 0.5f;

    private TextObject textObject;
    private SpriteRenderer spriteRenderer;
    private Color alphaColour;

    void Awake()
    {
        blackFilter = Instantiate(prefab_blackFilter, prefab_blackFilter.transform.position, prefab_blackFilter.transform.rotation) as GameObject;
        spriteRenderer = blackFilter.transform.FindChild("background").GetComponent<SpriteRenderer>();
        textObject = blackFilter.transform.FindChild("TextObject").GetComponent<TextObject>();
        alphaColour = Color.white;

        alpha = 1f;
        textObject.Hide();
        UpdateAlpha();
        
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeOut(bool caught)
    {
        alpha = 0f;
        textObject.Hide();
        UpdateAlpha();

        fadingIn = false;
        if (caught)
        {
            textObject.UpdateString("Try again tomorrow!");
        }
        else
        {
            textObject.UpdateString("END OF\nDAY " + Global.dayCount);
        }

        StartAnimation();
    }

    public void FadeIn()
    {
        alpha = 1f;
        textObject.Hide();
        UpdateAlpha();

        fadingIn = true;
        textObject.UpdateString("DAY " + Global.dayCount);

        StartAnimation();
    }


    private void StartAnimation()
    {
        Time.timeScale = 0;
        animationStartRealTime = Time.realtimeSinceStartup;
        animating = true;
    }

    private void Update()
    {
        if (animating)
        {
            float t = Time.realtimeSinceStartup - animationStartRealTime;
            if (fadingIn)
            {
                UpdateFadeIn(t);
            }
            else
            {
                UpdateFadeOut(t);
            }
        }
    }

    void UpdateFadeIn(float t)
    {
        if (t < 0.5f)
        {

        } else if (t < 1.5f)
        {
            textObject.Show();
        } else if (t < 2f)
        {
            textObject.Hide();
        } else if (t < 5f)
        {
            float f = (t - 2f)/3f;
            alpha = 1 - f;
            UpdateAlpha();
        }
        else
        {
            alpha = 0f;
            UpdateAlpha();
            textObject.Hide();
            animating = false;
            Time.timeScale = 1;
        }
    }

    private void UpdateFadeOut(float t)
    {
        if (t < 3f)
        {
            float f = t / 3f;
            alpha = f;
            UpdateAlpha();
        }
        else if (t < 3.5f)
        {
            alpha = 1f;
            UpdateAlpha();
        }
        else if (t < 4.5f)
        {
            textObject.Show();
        }
        else if (t < 5f)
        {
            textObject.Hide();
        }
        else
        {
            alpha = 1f;
            UpdateAlpha();
            textObject.Hide();
            Time.timeScale = 1;
            Global.RestartGame();
        }

    }

    void UpdateAlpha()
    {
        alphaColour.a = alpha;
        spriteRenderer.color = alphaColour;
    }



}