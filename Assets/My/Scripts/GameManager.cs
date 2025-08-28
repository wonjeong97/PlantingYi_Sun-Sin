using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Reporter reporter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Cursor.visible = false;
    }

    private void Update()
    {
        // D키를 눌러 디버그 패널 활성화 / 비활성화
        if (Input.GetKeyDown(KeyCode.D))
        {
            reporter.showGameManagerControl = !reporter.showGameManagerControl;

            if (reporter.show)
            {
                reporter.show = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Cursor.visible = !Cursor.visible;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}