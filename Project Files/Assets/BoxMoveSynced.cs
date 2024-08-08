using Photon.Pun;
using UnityEngine;

public class BoxMoveSynced : MonoBehaviour, IPunObservable
{
    public float moveSpd;
    private Vector2 networkedPosition;
    private Vector2 networkedVelocity;
    private float lerpSpeed = 10f;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && GetComponent<PhotonView>().IsMine)
        {
            // Handle box movement locally (Master Client)
            rb2D.velocity = Vector2.left * moveSpd;
        }
        else
        {
            // Smooth movement of the box with interpolation for remote clients
            rb2D.position = Vector2.Lerp(rb2D.position, networkedPosition, Time.deltaTime * lerpSpeed);
            rb2D.velocity = Vector2.Lerp(rb2D.velocity, networkedVelocity, Time.deltaTime * lerpSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the box's position and velocity to other clients
            stream.SendNext(rb2D.position);
            stream.SendNext(rb2D.velocity);
        }
        else
        {
            // Receive the box's position and velocity from the owner
            networkedPosition = (Vector2)stream.ReceiveNext();
            networkedVelocity = (Vector2)stream.ReceiveNext();
        }
    }
}