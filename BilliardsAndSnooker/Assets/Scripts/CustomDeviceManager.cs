using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomDeviceManager : MonoBehaviour
{
    public GameObject MainMenu;
    public bool LockAxisX;
    public bool LockAxisY;
    public bool LockAxisZ;
    public GameObject MenuInfo;
    public Transform Center;
    public Pool8Ball Table;
    [SerializeField] private TeleportationProvider provider;
    [SerializeField] XRNode LeftHand = XRNode.LeftHand;
    [SerializeField] XRNode RightHand = XRNode.RightHand;
    public GameObject LeftHandRayProjectileCurve;
    public GameObject LeftHandRayStreightLine;
    public GameObject LeftHandDirect;
    GameObject ActiveVariantOfHand;
    XRRayInteractor rayInteractor;

    private List<InputDevice> devices = new List<InputDevice>();

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private bool _isActive;
    bool _isActiveMenuButton = false;
    bool MenuInfoActive;
    

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(LeftHand, devices);
        leftDevice = devices.FirstOrDefault();

        InputDevices.GetDevicesAtXRNode(RightHand, devices);
        rightDevice = devices.FirstOrDefault();
    }

    void OnEnable()
    {
        if (!leftDevice.isValid || !rightDevice.isValid)
        {
            GetDevice();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor = LeftHandRayProjectileCurve.GetComponent<XRRayInteractor>();
        LeftHandRayProjectileCurve.SetActive(false);
        LeftHandDirect.SetActive(true);
        ActiveVariantOfHand = LeftHandDirect;

        _isActive = false;
        MenuInfoActive = true;
    }


    // Update is called once per frame

    void Update()
    {
        //RotationAfterTeleport();
        if (!leftDevice.isValid || !rightDevice.isValid)
        {
            GetDevice();
        }

        TeleportationPrimaryButton();
        MenuButton();       
    }

    public void HapticManager(string hand, float instesivity, float duration)
    {
        if(hand == "left")
        {
            if (!leftDevice.isValid)
            {
                GetDevice();
            }
            leftDevice.SendHapticImpulse(0, instesivity, duration);
        }

        if(hand == "right")
        {
            if (!rightDevice.isValid)
            {
                GetDevice();
            }
            rightDevice.SendHapticImpulse(0, instesivity, duration);
        }
    }

    void RotationAfterTeleport()
    {
        Vector3 direction = Center.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        if (LockAxisX)
        {
            rotation = new Quaternion(transform.rotation.x, rotation.y, rotation.z, rotation.w);
        }
        if (LockAxisY)
        {
            rotation = new Quaternion(rotation.x, transform.rotation.y, rotation.z, rotation.w);
        }
        if (LockAxisZ)
        {
            rotation = new Quaternion(rotation.x, rotation.y, transform.rotation.z, rotation.w);
        }
        transform.rotation = rotation;
    }

    void TeleportationPrimaryButton()
    {
        bool primaryButtonAction = false;
        if (leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonAction) && primaryButtonAction)
        {
            if (LeftHandDirect.activeSelf)
            {
                ActiveVariantOfHand = LeftHandDirect;
            }
            else if (LeftHandRayStreightLine.activeSelf)
            {
                ActiveVariantOfHand = LeftHandRayStreightLine;
            }
            if (!_isActive)
            {
                MenuInfoActive = MenuInfo.activeSelf;
                MenuInfo.SetActive(false);
                // ActiveVariantOfHand.SetActive(false);
                LeftHandRayProjectileCurve.SetActive(true);
                _isActive = true;
                HapticManager("left", 1f, 0.01f);
            }
        }
        else
        {
            if (_isActive)
            {
                // teleport
                rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);
                HapticManager("left", 1f, 0.02f);

                if (hit.collider.name == "Ground")
                {
                    TeleportRequest request = new TeleportRequest()
                    {
                        destinationPosition = hit.point,
                    };
                    provider.QueueTeleportRequest(request);
                    Invoke("RotationAfterTeleport", 0.1f);
                }
                LeftHandRayProjectileCurve.SetActive(false);
                //  ActiveVariantOfHand.SetActive(true);
                MenuInfo.SetActive(MenuInfoActive);
                _isActive = false;
            }
        }
    }

    void MenuButton()
    {
        bool menuButtonAction = false;
        if (leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonAction) && menuButtonAction)
        {

            if (!_isActiveMenuButton)
            {
                OpenCloseMenuButton();
            }
        }
        else
        {
            _isActiveMenuButton = false;
        }
    }

    public void OpenCloseMenuButton()
    {
        if (MainMenu.activeSelf)
        {
            MainMenu.SetActive(false);
            Table.ChangeHandToRay(false);
        }
        else
        {
            MainMenu.SetActive(true);
            Table.ChangeHandToRay(true);
        }
        _isActiveMenuButton = true;
    }
}