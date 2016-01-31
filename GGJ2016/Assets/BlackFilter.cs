using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackFilter : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_blackFilter = null;
    private GameObject blackFilter;

    [SerializeField]
    private Sprite spriteBlack;
    [SerializeField]
    private Sprite spriteCaught;

    private float alpha = 1f;
    private float alphaSpeed = 1.2f;

    private float fadedAlpha = 1f;

    private bool isCaught;
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
        isCaught = caught;
        if (caught)
        {
            spriteRenderer.sprite = spriteCaught;
            textObject.UpdateString("");
            //textObject.SetColour(Color.black);
            //textObject.SetLocalPosition(new Vector3(0, 2, 0));
            //textObject.UpdateString("Try again tomorrow!");
        }
        else
        {
            var vars = GameManager.Instance.GameVars;
            var ai = GameManager.Instance.Character.ai;
            
            var scores = new List<string>();

            textObject.SmallFont();

            if (vars.minutesPassed >= 60) scores.Add("Left the house past 8!");
            if (!ai.IsHolding(PickUpType.Briefcase)) scores.Add("Left house without Briefcase!");
            if (!ai.IsHolding(PickUpType.KeysWallet)) scores.Add("Left house without Key & Wallet!");
            if (!ai.IsHolding(PickUpType.Shoes)) scores.Add("Left house without Shoes!");
            if (!vars.changedClothes) scores.Add("Left house in pyjamas!");




            spriteRenderer.sprite = spriteBlack;
            textObject.UpdateString("END OF DAY " + Global.dayCount + "\nACHIEVEMENTS:\n\n" + string.Join("\n", scores.ToArray()));
        }

        StartAnimation();
    }

    public void FadeIn()
    {
        alpha = 1f;
        textObject.Hide();
        UpdateAlpha();

        spriteRenderer.sprite = spriteBlack;
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
                if (isCaught)
                {
                    UpdateCaught(t);
                }
                else
                {
                    UpdateFadeOut(t);
                }
            }
        }
    }


    void UpdateCaught(float t)
    {
        if (t < 0.5f)
        {
            float f = t / 0.5f;
            alpha = f;
            UpdateAlpha();
        }
        else if (t < 2.5f)
        {
            alpha = 1f;
            UpdateAlpha();
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
        } else if (t < 4f)
        {
            float f = (t - 2f)/2f;
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
        if (t < 2f)
        {
            float f = t / 2f;
            alpha = f;
            UpdateAlpha();
        }
        else if (t < 2.5f)
        {
            alpha = 1f;
            UpdateAlpha();
        }
        else if (t < 4f)
        {
            textObject.Show();
        }
        else if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
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