using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public Transform target; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Con la c�mara, usamos un Vector3, ya que la c�mara siempre sigue al jugador pero en un Z index distinto
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10f);

        //Cambiamos la posici�n de la c�mara basada en la del target
        //Usamos la funci�n slerp de Vector3, nos permite interpolaci�n entre 2 vectores, en �ste caso la ubicaci�n previa y la nueva
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }
}
