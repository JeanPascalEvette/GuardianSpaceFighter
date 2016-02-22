using UnityEngine;
using System.Collections;

public class GameInput : MonoBehaviour 
{
	public enum Direction { Up, Down, Left, Right };
	
	public delegate void OnTapCallback( Vector3 position );
    public delegate void OnSwipeCallback(Vector3 position);
    public delegate void OnTiltCallback(bool isLeft);

    public static event OnTapCallback OnTap;
    public static event OnTiltCallback OnTilt;
    public static event OnSwipeCallback OnSwipe;

	private enum MouseButtons
	{
		Left,
		Right,
		Middle,

		NumMouseButtons
	};

	private const int kNumMouseButtons = (int)MouseButtons.NumMouseButtons;

	private bool [] mMouseButton;
	private bool [] mMouseButtonLast;
    private bool mIsPotentiallyTapping;
    private float mStartTime;


    private Vector3 mStartPosition;
    private const float TapMoveThreshold = 50.0f;
	private const float TapDuration = 0.5f;
	private const float SwipeDuration = 1.0f;

	void Start () 
	{
		mMouseButton = new bool[kNumMouseButtons];
		mMouseButtonLast = new bool[kNumMouseButtons];
		for( int count = 0; count < kNumMouseButtons; count++ )
		{
			mMouseButton[count] = false;
			mMouseButtonLast[count] = false;
		}

		Input.simulateMouseWithTouches = true;
	}
	
	void Update () 
	{
		// Cache the last frame mouse status and read in the current mouse status 
		for( int count = 0; count < kNumMouseButtons; count++ )
		{
			mMouseButtonLast[count] = mMouseButton[count];
			mMouseButton[count] = Input.GetMouseButton( count );
		}

		bool tap = false;
		bool swipe = false;
        

        if (Input.GetKeyDown(KeyCode.Q) || Input.acceleration.x < -0.3f)
        {
            OnTilt(true);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.acceleration.x > 0.3f)
        {
            OnTilt(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        }

        // Detect different input types: Tap and Swipe
        if ( MouseButtonJustPressed( MouseButtons.Left ) )
		{
            mStartPosition = Input.mousePosition;
            mStartTime = Time.time;
            mIsPotentiallyTapping = true;

        }
		else if( MouseButtonHeld( MouseButtons.Left ) )
		{
            float duration = Time.time - mStartTime;
            mIsPotentiallyTapping = mStartPosition == Input.mousePosition && duration <= TapDuration;
            swipe = true;
        }
        else if (MouseButtonJustReleased(MouseButtons.Left))
        {
            if (mIsPotentiallyTapping)
            {
                tap = true;
            }
        }
        else
        {
            tap = false;
            swipe = false;
		}
			
		if( tap || swipe )
		{
			if( tap && OnTap != null )
			{
				OnTap( Input.mousePosition );
			}
			else if( swipe && OnSwipe != null )
            {
				OnSwipe(Input.mousePosition);
			}
		}
	}

	private bool MouseButtonJustPressed( MouseButtons button )
	{
		return mMouseButton[(int)button] && !mMouseButtonLast[(int)button];
	}

	private bool MouseButtonJustReleased( MouseButtons button )
	{
		return !mMouseButton[(int)button] && mMouseButtonLast[(int)button];
	}

	private bool MouseButtonHeld( MouseButtons button )
	{
		return mMouseButton[(int)button] && mMouseButtonLast[(int)button];
	}
}
