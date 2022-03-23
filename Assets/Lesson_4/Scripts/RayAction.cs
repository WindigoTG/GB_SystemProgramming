using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RayAction : FireAction
{
    private Camera camera;

    protected override void Start()
    {
        base.Start();
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reloading();
        }

        if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    protected override void Shooting()
    {
        if (!hasAuthority)
            return;

        base.Shooting();
        if (bullets.Count > 0)
        {
            //StartCoroutine(Shoot());

            if (reloading)
                return;

            var point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
            var ray = camera.ScreenPointToRay(point);

            Debug.Log(ray.origin + "  |  " + ray.direction);
            Debug.DrawRay(ray.origin, ray.direction);

            CmdShoot(ray.origin, ray.direction);
        }
    }

    [Command]
    private void CmdShoot(Vector3 origin, Vector3 direction)
    {
        if (!Physics.Raycast(origin, direction, out var hit))
        {
            return;
        }

        var damageable = hit.transform.GetComponentInParent<IDamageable>();
        Debug.Log(damageable);
        if (damageable != null)
        {
            damageable.TakeDamage(15);
        }

        RpcShoot(hit.point);
    }

    [ClientRpc]
    private void RpcShoot(Vector3 hit)
    {
        StartCoroutine(Shoot(hit));
    }


    private IEnumerator Shoot(Vector3 hit)
    {
        if (reloading)
        {
            yield break;
        }

        var shoot = bullets.Dequeue();
        bulletCount = bullets.Count.ToString();
        ammunition.Enqueue(shoot);
        shoot.SetActive(true);
        shoot.transform.position = hit;//.point;
        //shoot.transform.parent = hit;//.transform;

        yield return new WaitForSeconds(2.0f);
        shoot.SetActive(false);
    }
}

