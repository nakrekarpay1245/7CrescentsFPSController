using UnityEngine;
using UnityEngine.UI;
public class FakePersonController : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;

    [Header("KAMERA HAREKETİ")]
    #region Kamera Hareketi
    [Tooltip("Oyuncu altındaki kamera")]
    public Camera playerCamera;

    public float fieldOfView = 60f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 70f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    [Header("Kamera Yakınlaştırma")]
    #region Kamera Yakınlaştırma
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    private bool isZoomed = false;

    //float cameraRotation;
    #endregion
    #endregion

    #region Hareket
    [Header("HAREKET")]
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private bool isWalking = false;

    #region Sürat
    [Header("SÜRAT")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sürat Barı
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    private GameObject sprintBarParent;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;
    #endregion

    #region Jump
    [Header("ZIPLAMA")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    private bool isGrounded = false;
    #endregion

    #region Crouch
    [Header("ÇÖKME/EĞİLME")]
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;
    #endregion
    #endregion

    #region Kafa Sallama
    [Header("KAFA SALLAMA")]
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;
    private float timer = 0;
    #endregion

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();

        crosshairObject = GameObject.Find("Crosshair").GetComponent<Image>();

        playerCamera.fieldOfView = fieldOfView;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        sprintRemaining = sprintDuration;
        sprintCooldownReset = sprintCooldown;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        crosshairObject.gameObject.SetActive(true);        

        #region Sürat Barı
        sprintBarParent = sprintBarBG.transform.parent.gameObject;

        sprintBarBG.gameObject.SetActive(true);
        sprintBar.gameObject.SetActive(true);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        sprintBarWidth = screenWidth * sprintBarWidthPercent;
        sprintBarHeight = screenHeight * sprintBarHeightPercent;

        sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
        sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

        if (hideBarWhenFull)
        {
            sprintBarParent.SetActive(false);
        }
        #endregion
    }

    private void Update()
    {
        #region Kamera
        // Kamera hareketi yönetimi        
        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        
        // Bakılan yerin değişimi
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        //}

        #region Kamera Yakınlaştırma       
        // Yakınlaştırma için tuşa basma
        if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
        {
            if (!isZoomed)
            {
                isZoomed = true;
            }
            else
            {
                isZoomed = false;
            }
        }

        //Yakınlaştırma için basılı tutma 
        if (holdToZoom && !isSprinting)
        {
            if (Input.GetKeyDown(zoomKey))
            {
                isZoomed = true;
            }
            else if (Input.GetKeyUp(zoomKey))
            {
                isZoomed = false;
            }
        }

        // Kamera görüş açısını pürüssüz(smooth-yavaşça) değiştirir
        if (isZoomed)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,
                zoomFOV, zoomStepTime * Time.deltaTime);
        }
        else if (!isZoomed && !isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,
                fieldOfView, zoomStepTime * Time.deltaTime);
        }
        #endregion
        #endregion

        #region Sürat        
        if (isSprinting)
        {
            isZoomed = false;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,
                sprintFOV, sprintFOVStepTime * Time.deltaTime);

            //Sürat sırasında sürat'i belli bir süre sonra kapatın            
            sprintRemaining -= 1 * Time.deltaTime;
            if (sprintRemaining <= 0)
            {
                isSprinting = false;
                isSprintCooldown = true;
            }
        }
        else
        {
            // Sürat yapmayı aktifleştirir 
            sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime,
                0, sprintDuration);
        }

        // Sürat bekleme süresini yönetir
        // Sürat bekleme süresi <= 0 ise sürat yapmayı engeller
        if (isSprintCooldown)
        {
            sprintCooldown -= 1 * Time.deltaTime;
            if (sprintCooldown <= 0)
            {
                isSprintCooldown = false;
            }
        }
        else
        {
            sprintCooldown = sprintCooldownReset;
        }

        // Sürat Barı Yönetir        
        float sprintRemainingPercent = sprintRemaining / sprintDuration;
        sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        #endregion

        #region Zıplama
        // Zıplama için girdi(Input) alır ve zıplatır         
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        #endregion

        #region Crouch        
        if (Input.GetKeyDown(crouchKey) && !holdToCrouch)
        {
            Crouch();
        }

        if (Input.GetKeyDown(crouchKey) && holdToCrouch)
        {
            isCrouched = false;
            Crouch();
        }
        else if (Input.GetKeyUp(crouchKey) && holdToCrouch)
        {
            isCrouched = true;
            Crouch();
        }
        #endregion

        CheckGround();
        
        HeadBob();
    }

    void FixedUpdate()
    {
        #region Hareket
        // Hareket girdilerini(Input) al
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"),
            0, Input.GetAxis("Vertical"));

        // Oyuncunun yürüyüp yürümediğini ve yerde olup olmadığını kontrol eder
        // Kafa sallanması için
        if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        // Sürat anındaki hareket
        if (Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

            // Hareket için rigidboy' e kuvvet uygular
            Vector3 velocity = rigidbodyComponent.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            // Oyuncu yalnızca hız değiştiğinde hareket eder != 0 
            // Görüş açısı değişikliğinin yalnızca hareket sırasında olmasını sağlar
            if (velocityChange.x != 0 || velocityChange.z != 0)
            {
                isSprinting = true;
                if (isCrouched)
                {
                    Crouch();
                }

                if (hideBarWhenFull/* && !unlimitedSprint*/)
                {
                    sprintBarParent.SetActive(true);
                }
            }

            rigidbodyComponent.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        // Yürüme anındaki hareket
        else
        {
            isSprinting = false;

            if (hideBarWhenFull && sprintRemaining == sprintDuration)
            {
                sprintBarParent.SetActive(false);
            }

            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

            // Hareket için rigidboy' e kuvvet uygular
            Vector3 velocity = rigidbodyComponent.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rigidbodyComponent.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        //}
        #endregion
    }

    // Raycast ile yerde olup olmadığımız kontrol edilir
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x,
            transform.position.y - (transform.localScale.y * 0.5f), transform.position.z);

        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
            //Debug.Log("is Grounded True");
        }
        else
        {
            isGrounded = false;
            //Debug.Log("is Grounded False");
        }
    }

    private void Jump()
    {
        // Oyuncuya zıplaması için kuvvet uygular
        if (isGrounded)
        {
            rigidbodyComponent.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
            //Debug.Log("is Grounded False");
        }

        if (isCrouched /*&& !holdToCrouch*/)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        // Oyuncuyu eski boyutuna yükseltir
        // walkSpeed'i orijinal hızına geri getirir
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;

            isCrouched = false;
        }
        // Çökme/Eğilme anında oyuncunun boyutunu ufalt/küçült
        // Yürüyüş Hızını Azalt
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;

            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            // Sürat sırasında kafa sallanması hızını hesaplar
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Çökme/Eğilme sırasında kafa sallanması hızını hesaplar
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Yürüme sırasında kafa sallanması hızını hesaplar
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            // Kafa sallanması hareketini uygular
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x,
                jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z +
                Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Oyuncu hareket etmeyi bıraktığında sıfırlanır
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x,
                Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y,
                Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z,
                Time.deltaTime * bobSpeed));
        }
    }
}