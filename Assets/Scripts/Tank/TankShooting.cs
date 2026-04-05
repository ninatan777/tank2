using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    // public variables, can be set in the unity inspector
    public GameObject camara;
    public int player_number = 1;
    public GameObject shell;                // 炮弹
    public GameObject FireTransform;        // 开火位置
    //public Slider aim_slider;
    public AudioSource shooting_audio;
    public AudioClip charging_clip;
    public AudioClip fire_clip;
    public float min_launch_force = 15f;
    public float max_launch_force = 30f;
    public float max_charge_time = 0.75f;

    private string fire_button;
    private float current_launch_force;     // 蓄力
    private float charge_speed;
    private bool fired;


    private void OnEnable()
    {
        current_launch_force = min_launch_force;
        //aim_slider.value = min_launch_force;
    }


    private void Start()
    {
        fire_button = "Fire" + player_number;
        charge_speed = (max_launch_force - min_launch_force) / max_charge_time;
    }

    private void Update()
    {
        if(current_launch_force >= max_launch_force && !fired)
        {
            // 后续添加炸膛效果
            current_launch_force = max_launch_force;
            Fire();
        }
        // 仅会在按下按钮的一瞬间的那一帧进入
        else if(Input.GetButtonDown(fire_button))
        {
            fired = false;
            current_launch_force = min_launch_force;
            shooting_audio.clip = charging_clip;
            shooting_audio.Play();
        }
        // 蓄力
        else if(Input.GetButton(fire_button) && !fired)
        {
            current_launch_force += charge_speed * Time.deltaTime;
            //aim_slider.value = current_launch_force;
        }
        // 没有到最大速度，直接发射
        else if(Input.GetButtonUp(fire_button) && !fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        fired =  true;
        GameObject shell_instance = Instantiate(shell, FireTransform.transform.position, FireTransform.transform.rotation) as GameObject;
        Rigidbody rb = shell_instance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = shell_instance.AddComponent<Rigidbody>();
        }
        rb.velocity = current_launch_force * FireTransform.transform.forward;
        shooting_audio.clip = fire_clip;
        shooting_audio.Play ();
        // reset force
        current_launch_force = min_launch_force;
    }
}