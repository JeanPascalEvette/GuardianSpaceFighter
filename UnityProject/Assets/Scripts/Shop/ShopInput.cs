using UnityEngine;
using System.Collections;

public class ShopInput : MonoBehaviour {

    public delegate void OnTapCallback(Vector3 position);

    public static event OnTapCallback OnTap;
    private enum MouseButtons
    {
        Left,
        Right,
        Middle,

        NumMouseButtons
    };

    private const int kNumMouseButtons = (int)MouseButtons.NumMouseButtons;


    private bool[] mMouseButton;
    private bool[] mMouseButtonLast;

    // Use this for initialization
    void Start () {

        mMouseButton = new bool[kNumMouseButtons];
        mMouseButtonLast = new bool[kNumMouseButtons];
        for (int count = 0; count < kNumMouseButtons; count++)
        {
            mMouseButton[count] = false;
            mMouseButtonLast[count] = false;
        }

        Input.simulateMouseWithTouches = true;
    }

    void Update()
    {
        
        for (int count = 0; count < kNumMouseButtons; count++)
        {
            mMouseButtonLast[count] = mMouseButton[count];
            mMouseButton[count] = Input.GetMouseButton(count);
        }
        

        // Using JustReleased to prevent the tapping from affecting the next scene as well.
        if (MouseButtonJustReleased(MouseButtons.Left))
        {
            OnTap(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        }
    }

    private bool MouseButtonJustReleased(MouseButtons button)
    {
        return !mMouseButton[(int)button] && mMouseButtonLast[(int)button];
    }
}
