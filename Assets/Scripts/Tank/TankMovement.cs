using UnityEngine;

public class TankMovement : MonoBehaviour
{
    // public variables, can be set in the unity inspector
    public int player_number = 1;               // player num, and will be used to determine input axis
    public float speed = 12f;                   // forward and back speed
    public float turn_speed = 180f;             // turning speed in degrees per second
    public AudioSource movement_audio;          // audio source, 定义的是一个播放器，能够设置各种播放时的参数和行为
    public AudioClip engine_idling;             // clip 定义的是播放的音频文件，此处为引擎空转声
    public AudioClip engine_driving;      
    public float pitch_range = 0.2f;            // 音调变化频率

    public Camera follow_camera;

    private string movement_axis_name;          // axis for forward backward movement, will be based on player number
    private string turn_axis_name;              // axis for turning, will be based on player number
    private Rigidbody rigidbody_component;
    private float movement_input_value;
    private float turn_input_value;
    private float original_pitch;               // 初始音调
    private ParticleSystem[] particleSystems;   // References to all the particles systems used by the Tanks
    private Vector3 offset;                     // 摄像机与坦克的相对位置


    // 生命周期
    // 脚本加载时调用一次，获取刚体组件即可
    private void Awake()
    {
        rigidbody_component = GetComponent<Rigidbody>();
    }

    // 当脚本被启用时调用，重置输入值，使刚体受物理引擎影响
    private void OnEnable ()
    {
        Debug.Log("[OnEnable] 设置刚体为非运动学，重置输入值");
        rigidbody_component.isKinematic = false;
        movement_input_value = 0f;
        turn_input_value = 0f;

        particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; ++i)
            {
                particleSystems[i].Play();
            }
    }

    // 当脚本被禁用时调用，使刚体不受物理引擎影响
    // 不会响应物理碰撞和力
    private void OnDisable ()
    {
        Debug.Log("[OnDisable] 设置刚体为运动学");
        rigidbody_component.isKinematic = true;
    }


    private void Start()
    {
        movement_axis_name = "Vertical" + player_number;
        turn_axis_name = "Horizontal" + player_number;
        original_pitch = movement_audio.pitch;
        Debug.Log($"[Start] movement_axis_name: {movement_axis_name}, turn_axis_name: {turn_axis_name}, original_pitch: {original_pitch}");

        if (follow_camera != null)
        {
            offset = follow_camera.transform.position - transform.position;
        }
    }

    // 适应机器性能的调用，适合输入/画面相关的更新
    private void Update()
    {
        movement_input_value = Input.GetAxis(movement_axis_name);
        turn_input_value = Input.GetAxis(turn_axis_name);
        Debug.Log($"[Update] movement_input_value: {movement_input_value}, turn_input_value: {turn_input_value}");
        EngineAudio();

        // 摄像机跟随逻辑
        if (follow_camera != null)
        {
            follow_camera.transform.position = transform.position + offset;
            follow_camera.transform.LookAt(transform.position);
        }
    }

    private void EngineAudio()
    {
        //  静止时播放idling audio
        if (Mathf.Abs(movement_input_value) < 0.1f && Mathf.Abs(turn_input_value) < 0.1f)
        {
            // 切歌
            if (movement_audio.clip == engine_driving)
            {
                movement_audio.clip = engine_idling;
                movement_audio.pitch = Random.Range(original_pitch - pitch_range, original_pitch + pitch_range);
                movement_audio.Play();
            }
        }
        else
        {
            if (movement_audio.clip == engine_idling)
            {
                movement_audio.clip = engine_driving;
                movement_audio.pitch = Random.Range(original_pitch - pitch_range, original_pitch + pitch_range);
                movement_audio.Play();
            }

        }
    }

    // 固定间隔调用，适合处理物理相关的更新
    private void FixedUpdate()
    {
        Move();
        Turn();
    }


    private void Move()
    {
        Vector3 movement = transform.forward * movement_input_value * speed * Time.deltaTime;
        Debug.Log($"[Move] movement: {movement}, position(before): {rigidbody_component.position}");
        rigidbody_component.MovePosition(rigidbody_component.position + movement);
        Debug.Log($"[Move] position(after): {rigidbody_component.position + movement}");
    }


    private void Turn()
    {
        float turn = turn_input_value * turn_speed * Time.deltaTime;
        Quaternion turn_rotation = Quaternion.Euler(0f, turn, 0f);
        Debug.Log($"[Turn] turn: {turn}, rotation(before): {rigidbody_component.rotation.eulerAngles}");
        rigidbody_component.MoveRotation(rigidbody_component.rotation * turn_rotation);
        Debug.Log($"[Turn] rotation(after): {(rigidbody_component.rotation * turn_rotation).eulerAngles}");
    }
}