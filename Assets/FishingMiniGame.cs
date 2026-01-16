
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;

    [SerializeField] Transform fish;
    [SerializeField] Item fishItem;

    float fishPosition = 0.5f;
    float fishDestination;

    float fishTimer;
    float fishSpeed ;
    [SerializeField] float smoothMotion = 1f;   
    [SerializeField] float timerMultiplicator = 3f;

    [SerializeField] Transform hook;
    float hookPosition;
    [SerializeField] float hookSize = 0.1f;
    [SerializeField] float hookPower = 0.5f;
    float hookProgress;
    float hookPullVelocity;
    [SerializeField] float hookPullPower = 0.01f;
    [SerializeField] float hookGravityPower = 0.005f;
    [SerializeField] float hookProgressDegradationPower = 0.1f;

    [SerializeField] Image hookSpriteRenderer;

    [SerializeField] Transform progressBarContainer;
    [SerializeField] float failTimer = 10f;
    bool pause = false;
    [SerializeField] TextMeshProUGUI FishingResultPanel;
    private void Start()
    {
        Resize();
    }    
    private void Update()
    {
        if (pause)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Fish();
            Hook();
            ProgressCheck();
        }
    }
    private void Resize()
    {
        RectTransform rectTransform = hookSpriteRenderer.GetComponent<RectTransform>();

        float ySize = rectTransform.rect.height;
        Vector3 ls = rectTransform.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        ls.y = (distance / ySize) * hookSize;
        rectTransform.localScale = ls;
       /* float ySize = bounds.size.y;
        Vector3 ls = hook.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        ls.y = (distance / ySize * hookSize);
        hook.localScale = ls;*/
    }

    private void ProgressCheck()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if(min < fishPosition && fishPosition < max)
        {
            hookProgress += hookPower * Time.deltaTime;
        }
        else
        {
            hookProgress -= hookProgressDegradationPower * Time.deltaTime;

            failTimer -= Time.deltaTime;
            if(failTimer < 0f)
            {
                Lose();
            }
        }
        if(hookProgress >= 1f) 
        {
            Win();
        }
        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    private void Lose()
    {
        pause = true;
        FishingResultPanel.text = string.Format("YOU LOSE! FISH SWIM AWAY FROM YOU!");
        //FishingResultPanel.gameObject.SetActive(true);
        Debug.Log("YOU LOSE! FISH SWIM AWAY FROM YOU!"); 
    }

    private void Win()
    {
        pause = true;
        Debug.Log("YOU WIN! YOU CAUGHT THE FISH!");
        FishingResultPanel.text = string.Format("YOU WIN! YOU CAUGHT THE FISH!");
        GameManager.Instance.inventoryContainer.Add(fishItem, 1);
    }

    void Hook()
    {
        if (Input.GetMouseButton(0))
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        hookPullVelocity -= hookGravityPower * Time.deltaTime;
        hookPosition += hookPullVelocity;
        if(hookPosition - hookSize <= 0f && hookPullVelocity < 0f)
        {
            hookPullVelocity = 0f;
        }
        if(hookPosition  + hookSize>= 1f && hookPullVelocity > 0f)
        {
            hookPullVelocity = 0f;
        }
        hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1 - hookSize/2);
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
    }
    private void Fish()
    {
        fishTimer -= Time.deltaTime;
        if(fishTimer < 0f)
        {
            fishTimer = Random.value * timerMultiplicator * 0.5f;

            fishDestination = Random.value;
        }

        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fishPosition = Mathf.Clamp(fishPosition, hookSize, 1 - hookSize);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }
}
