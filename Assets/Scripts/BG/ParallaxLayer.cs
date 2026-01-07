using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.5f; // 0 = không di chuyển, 1 = di chuyển cùng camera

    private float startPositionX;
    private float spriteWidth;
    private GameObject[] backgrounds;

    void Start()
    {
        startPositionX = transform.position.x;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        // Tạo 3 bản sao cho smooth scrolling
        backgrounds = new GameObject[3];
        backgrounds[0] = gameObject;

        for (int i = 1; i < 3; i++)
        {
            backgrounds[i] = Instantiate(gameObject, transform.parent);
            backgrounds[i].name = gameObject.name + "_" + i;
            backgrounds[i].transform.position = new Vector3(
                startPositionX + spriteWidth * i,
                transform.position.y,
                transform.position.z
            );
            //Destroy(backgrounds[i].GetComponent<CameraFollowParallax>());
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Tính parallax offset
        float parallaxOffset = (cameraTransform.position.x - startPositionX) * parallaxEffect;

        // Di chuyển background
        foreach (var bg in backgrounds)
        {
            if (bg == null) continue;

            float newX = startPositionX + parallaxOffset;
            bg.transform.position = new Vector3(newX, bg.transform.position.y, bg.transform.position.z);

            // Loop lại khi cần
            if (cameraTransform.position.x - bg.transform.position.x > spriteWidth)
            {
                bg.transform.position = new Vector3(
                    bg.transform.position.x + spriteWidth * 3,
                    bg.transform.position.y,
                    bg.transform.position.z
                );
                startPositionX += spriteWidth * 3;
            }
            else if (bg.transform.position.x - cameraTransform.position.x > spriteWidth)
            {
                bg.transform.position = new Vector3(
                    bg.transform.position.x - spriteWidth * 3,
                    bg.transform.position.y,
                    bg.transform.position.z
                );
                startPositionX -= spriteWidth * 3;
            }
        }
    }
}