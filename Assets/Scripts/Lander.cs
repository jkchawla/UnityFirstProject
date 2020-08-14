using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lander : MonoBehaviour
{
    public bool allowThrust;

    public Transform bottomThruster;

    public GameObject explosionPrefab;

    public float fuel;

    public Text fuelText;

    public Transform leftThruster;

    public Animator leftThrusterAnim;

    public Animator mainThrusterAnim;

    public float mainThrustPower;

    public Transform rightThruster;

    public Animator rightThrusterAnim;

    public float sideThrustPower;

    public AudioSource thrusterAudio;

    private bool canDeployFeet;

    private GameObject landerObjective;

    private Rigidbody2D landerRigidBody2D;

    private HingeJoint2D feetJoint;

    private Button restartbutton;

    private bool shouldPlayThrustSfx;

    // Use this for initialization
    void Start()
    {
        landerRigidBody2D = GetComponent<Rigidbody2D>();
        landerObjective = GameObject.Find("LanderObjective");
        feetJoint = transform.FindChild("LanderFeet").GetComponent<HingeJoint2D>();
        restartbutton = GameObject.Find("RestartButton").GetComponent<Button>();
        restartbutton.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
        restartbutton.gameObject.GetComponent<Image>().enabled = false;
        restartbutton.gameObject.transform.FindChild("Text").GetComponent<Text>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var objectiveDistance = Vector2.Distance(transform.position, landerObjective.transform.position);

        if (objectiveDistance <= 1f && canDeployFeet == false)
        {
            canDeployFeet = true;
        }

        if (canDeployFeet)
        {
            var landerFeetYPos = feetJoint.anchor.y + Time.deltaTime / 3;

            if (landerFeetYPos < 0.38f)
            {
                feetJoint.anchor = new Vector2(0f, landerFeetYPos);
            }
            else
            {
                canDeployFeet = false;
            }
        }

        if (shouldPlayThrustSfx)
        {
            PlayThrusterSfx();
        }
        else
        {
            thrusterAudio.Pause();
        }
    }

    void FixedUpdate()
    {
        shouldPlayThrustSfx = false;

        if (Input.GetAxis("Vertical") > 0)
        {
            shouldPlayThrustSfx = true;
            ApplyForce(bottomThruster, mainThrustPower);
            if (mainThrusterAnim != null && mainThrusterAnim.runtimeAnimatorController != null)
            {
                mainThrusterAnim.SetBool("ApplyingThrust", true);
            }
        }
        else
        {
            if (mainThrusterAnim != null && mainThrusterAnim.runtimeAnimatorController != null)
            {
                mainThrusterAnim.SetBool("ApplyingThrust", false);
            }
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            shouldPlayThrustSfx = true;
            ApplyForce(leftThruster, sideThrustPower);
            if (leftThrusterAnim != null && leftThrusterAnim.runtimeAnimatorController != null)
            {
                leftThrusterAnim.SetBool("ApplyingThrust", true);
            }
        }
        else
        {
            if (leftThrusterAnim != null && leftThrusterAnim.runtimeAnimatorController != null)
            {
                leftThrusterAnim.SetBool("ApplyingThrust", false);
            }
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            shouldPlayThrustSfx = true;
            ApplyForce(rightThruster, sideThrustPower);
            if (rightThrusterAnim != null && rightThrusterAnim.runtimeAnimatorController != null)
            {
                rightThrusterAnim.SetBool("ApplyingThrust", true);
            }
        }
        else
        {
            if (rightThrusterAnim != null && rightThrusterAnim.runtimeAnimatorController != null)
            {
                rightThrusterAnim.SetBool("ApplyingThrust", false);
            }
        }
    }

    private void PlayThrusterSfx()
    {
        if (thrusterAudio.isPlaying)
        {
            return;
        }

        thrusterAudio.Play();
    }

    private void ApplyForce(Transform thrusterTransform, float thrustPower)
    {
        if (allowThrust && fuel > 0f)
        {
            var direction = transform.position - thrusterTransform.position;
            landerRigidBody2D.AddForceAtPosition(direction.normalized * thrustPower, thrusterTransform.position);
            fuel -= 0.01f;
            fuelText.text = "Fuel " + Mathf.Round(fuel);
        }
    }

    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.relativeVelocity.magnitude > 3)
        {
            HandleLanderDestroy();
        }
        else if (hitInfo.relativeVelocity.magnitude > 1)
        {
            HandleLanderDestroy();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Fuel")
        {
            fuel += 10f;
            Destroy(collider.gameObject);
        }
    }

    private void HandleLanderDestroy()
    {
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        Destroy(gameObject);
        restartbutton.gameObject.GetComponent<Image>().enabled = true;
        restartbutton.gameObject.transform.FindChild("Text").GetComponent<Text>().enabled = true;
    }

    public void EnableRestartButton()
    {
        restartbutton.gameObject.GetComponent<Image>().enabled = true;
        restartbutton.gameObject.transform.FindChild("Text").GetComponent<Text>().enabled = true;
    }
}