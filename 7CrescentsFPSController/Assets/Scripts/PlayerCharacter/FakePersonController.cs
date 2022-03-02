using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FakePersonController : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fieldOfView = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    //public Sprite crosshairImage;
    // public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    //public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    float cameraRotation;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;

    #region Sprint

    //public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    //public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private GameObject sprintBarParent;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    //public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    #endregion

    #region Crouch

    //public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();

        crosshairObject = GameObject.Find("Crosshair").GetComponent<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fieldOfView;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            //crosshairObject.sprite = crosshairImage;
            //crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarParent = sprintBarBG.transform.parent.gameObject;

        //if (useSprintBar)
        //{
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
        //}
        //else
        //{
        //    sprintBarBG.gameObject.SetActive(false);
        //    sprintBar.gameObject.SetActive(false);
        //}

        #endregion
    }

    private void Update()
    {
        #region Camera

        // Kamera hareketi yönetimi
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Bakılan yerin değişimi
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        //if (enableZoom)
        //{
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
        //}

        #endregion
        #endregion

        #region Sprint

        //if (enableSprint)
        //{
        if (isSprinting)
        {
            isZoomed = false;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,
                sprintFOV, sprintFOVStepTime * Time.deltaTime);

            //Sürat sırasında sürat'i belli bir süre sonra kapatın
            if (!unlimitedSprint)
            {
                sprintRemaining -= 1 * Time.deltaTime;
                if (sprintRemaining <= 0)
                {
                    isSprinting = false;
                    isSprintCooldown = true;
                }
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

        // Süra tBarı Yönetir 
        //if (useSprintBar && !unlimitedSprint)
        //{
        if (!unlimitedSprint)
        {
            float sprintRemainingPercent = sprintRemaining / sprintDuration;
            sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        }
        //}

        #endregion

        #region Jump

        // Zıplama için girdi(Input) alır ve zıplatır
        //if (enableJump && Input.GetKeyDown(jumpKey) && isGrounded)
        //{  
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        #endregion

        #region Crouch

        //if (enableCrouch)
        //{
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
        //}

        #endregion

        CheckGround();

        if (enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
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
            //if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
            //{ 
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

                    if (hideBarWhenFull && !unlimitedSprint)
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
        }

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

        if (isCrouched && !holdToCrouch)
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
            // Yürüme sırasında kaa sallanması hızını hesaplar
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



// Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(FakePersonController)), InitializeOnLoadAttribute]
public class FakePersonControllerEditor : Editor
{
    FakePersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc = (FakePersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Fake Person Controller", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Erkan Yaprak", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 0.0.1", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Kamera Ayarları", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new
            GUIContent("Kamera", "Oyuncunun altındaki kamera"),
            fpc.playerCamera, typeof(Camera), true);

        fpc.fieldOfView = EditorGUILayout.Slider(new
            GUIContent("Görüş Açısı", "Kamera' nın görüş açısı " +
            "Oyuncu kamerasını değiştirir"), fpc.fieldOfView, fpc.zoomFOV, 179f);

        fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Kamera Dönüşünü Aktif Eder",
            "Kameranın hareket etmesine izin verilip verilmeyeceğini belirler."), fpc.cameraCanMove);

        GUI.enabled = fpc.cameraCanMove;
        fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Kamera Dönüşünü Ters Çevir",
            "Kameranın yukarı ve aşağı hareketini tersine çevirir."), fpc.invertCamera);

        fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Bakma Hassasiyeti",
            "Fare hareketinin ne kadar hassas olduğunu belirler."), fpc.mouseSensitivity, .1f, 10f);

        fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Maksimum Bakış Açısı",
            "Oyuncu kamerasının bakabileceği maksimum ve minimum açıyı belirler."),
            fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("İmleci Kilitle ve Gizle",
            "İmleç görünürlüğünü kapatır ve ekranın ortasına kilitler."),
            fpc.lockCursor);

        fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Otomatik Crosshair",
            "Temel crosshairin açılıp açılmayacağını belirler ve ekranın ortasına ayarlar."),
            fpc.crosshair);

        // Yalnızca Crosshair aktifse görünür
        //if (fpc.crosshair)
        //{
        //    EditorGUI.indentLevel++;
        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Görseli",
        //        "Crosshair olarak kullanılacak Sprite"));
        //    EditorGUILayout.EndHorizontal();

        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.EndHorizontal();
        //    EditorGUI.indentLevel--;
        //}

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Yakınlaştırma", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 13
        }, GUILayout.ExpandWidth(true));

        //fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Yakınlaştırmayı Etkinleştir",
        //    "Oyuncunun oynarken yakınlaştırma yapıp yapamayacağını belirler."), fpc.enableZoom);

        //GUI.enabled = fpc.enableZoom;
        fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Yakınlaştırma İçin Basılı Tut",
            "Yakınlaştırmak ve yakınlaştırmayı tutmak için oyuncunun yakınlaştırma" +
            " tuşunu basılı tutmasını gerektirir."),
            fpc.holdToZoom);

        fpc.zoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Yakınlaştırma Tuşu",
            "Yakınlaştırmak için hangi tuşun kullanılacağını belirler."), fpc.zoomKey);

        fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Yakınlaştırma Görüş Açısı",
            "Kameranın yakınlaştırıldığında görüş alanını belirler."),
            fpc.zoomFOV, .1f, fpc.fieldOfView);

        fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Yakınlaştırma Süresi",
            "Yakınlaştırma sırasında Görüş Açıları arası geçişlerinin ne kadar hızlı" +
            " olacağını belirler."), fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Hareket Ayarları", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));

        EditorGUILayout.Space();

        fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Oyuncu Hareketini Etkinleştir",
            "Oyuncunun hareket etmesine izin verilip verilmeyeceğini belirler."), fpc.playerCanMove);

        GUI.enabled = fpc.playerCanMove;

        fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Yürüme Hızı",
            "Yürürken oyuncunun ne kadar hızlı hareket edeceğini belirler."),
            fpc.walkSpeed, .1f, fpc.sprintSpeed);

        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sürat", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));

        //fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Sürat' i Aktifleştir",
        //    "Oyuncunun koşmasına izin verilip verilmeyeceğini belirler."), fpc.enableSprint);

        //GUI.enabled = fpc.enableSprint;
        fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Sınırsız Sürat",
            "'Sürat Süresi'nin etkin olup olmadığını belirler." +
            " Bunu açmak, sınırsız sürat koşusuna izin verecektir."), fpc.unlimitedSprint);

        fpc.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sürat Tuşu",
            "Sürat için hangi tuşun kullanılacağını belirler"), fpc.sprintKey);

        fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sürat Hızı",
            "Oyuncunun süratli iken ne kadar hızlı hareket edeceğini belirler."),
            fpc.sprintSpeed, fpc.walkSpeed, 20f);

        GUI.enabled = !fpc.unlimitedSprint;
        fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sürat Süresi",
            "Sınırsız sürat devre dışıyken oyuncunun ne kadar süre sürat yapabileceğini belirler."),
            fpc.sprintDuration, 1f, 20f);

        fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Süret Bekleme Süresi",
            "Oyuncunun sürat süresi bittiğinde ne kadar zaman sonra tekrar sürat yapabileceğini belirler"),
            fpc.sprintCooldown, .1f, fpc.sprintDuration);
        GUI.enabled = true;

        fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sürat Görüş Açısı",
            "Sprint sırasında kameranın görüş alanını belirler."),
            fpc.sprintFOV, fpc.fieldOfView, 179f);

        fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Yakınlaştırma Süresi",
            "Yakınlaştırma sırasında Görüş Açıları arası geçişlerinin ne kadar hızlı" +
            " olacağını belirler."),
            fpc.sprintFOVStepTime, .1f, 20f);

        //fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Sürat Bar' ı Kullan",
        //    "Varsayılan sürat barının ekranda görünüp görünmeyeceğini belirler."), fpc.useSprintBar);

        //// Yalnızca sprint çubuğu etkinleştirilmişse görünür
        //if (fpc.useSprintBar)
        //{
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Tam Çubuğu Gizle",
                "Sürat süresi dolduğunda sürat barını gizler ve sürat yaparken barı soluklaştırır. " +
                "Bunu devre dışı bırakmak, barın her zaman ekranda kalmasına neden olur." +
                " Sürat barı etkinleştirildi."), fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar Arkaplanı",
                "Sürat Barı arka planı olarak kullanılacak nesne."));

            fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.sprintBarBG,
            typeof(Image), true);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Sürat Barı olarak kullanılacak nesne"));

            fpc.sprintBar = (Image)EditorGUILayout.ObjectField(fpc.sprintBar, typeof(Image), true);

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Genişliği",
                "Barın genişliğini belirtir"), fpc.sprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Yüksekliği",
                "Barın yüksekliğini belirtir"), fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        //}
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Zıplama Ayarları", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));

        //fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Zıplamayı Aktifleştir",
        //    "Oyuncunun zıplamasına izin verilip verilmeyeceğini belirler."), fpc.enableJump);

        //GUI.enabled = fpc.enableJump;
        fpc.jumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zıplama Tuşu",
            "Zıplamak için hangi tuşun kullanılacağını belirler"), fpc.jumpKey);

        fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Zıplama Gücü",
            "Oyuncunun ne kadar yükseğe zıplayacağını belirler."), fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Çökme/Eğilme", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));

        //fpc.enableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Çökme/Eğilme 'yi aktifleştir",
        //    "Oyuncunun çökme/eğilme yapıp yapamayacağını belirler"), fpc.enableCrouch);

        //GUI.enabled = fpc.enableCrouch;

        fpc.holdToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Çökme/Eğilme için Basılı Tut",
            "Çökme/Eğilme için ve o halde kalmak için oyuncunun çökme/eğilme" +
            " tuşunu basılı tutmasını gerektirir."),
            fpc.holdToCrouch);

        fpc.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Çökme/Eğilme Tuşu",
            "Çömelmek için hangi tuşun kullanılacağını belirler"), fpc.crouchKey);

        fpc.crouchHeight = EditorGUILayout.Slider(new GUIContent("Çökme/Eğilme Yüksekliği",
            "Çökme/Eğilme anında oyuncunun boyunu belirler."), fpc.crouchHeight, .1f, 1);

        fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Hız Azaltma",
            "'Yürüme Hızı'nın azaltıldığı yüzdeyi belirler. 1 azalma yok ve 0,5 ise yarım."),
            fpc.speedReduction, .1f, 1);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Kafa Sallanması (Kamera Yürüme Efekti)", new GUIStyle(GUI.skin.label)
        { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 },
        GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Kafa Sallanmasını Aktifleştir",
            "Oyuncu yürürken kameranın sallanıp sallanmayacağını belirler."), fpc.enableHeadBob);


        GUI.enabled = fpc.enableHeadBob;

        fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Kamera Bağlantısı",
            "***"), fpc.joint,
            typeof(Transform), true);

        fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Kafa SAllanma Hızı",
            "Kafa sallanması halinde kamera sallanma hızını belirler"), fpc.bobSpeed, 1, 20);

        fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Kafa SAllanma Ömiktarı",
            "Kafanın her iki yönde ne kadar hareket edeceğini belirler"),
            fpc.bobAmount);

        GUI.enabled = true;

        #endregion        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }

}

#endif