using System;
using System.Collections;
using UnityEngine;

public class DropLock : MonoBehaviour
{
	public Part_Stage part;

	public StagePause stagePause;

	private IEnumerator Start()
	{
		while (part == null || stagePause == null)
		{
			yield return null;
		}
		Transform myTrans = base.transform;
		System.Random rand = new System.Random();
		Rigidbody rb = base.gameObject.AddComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeAll;
		float xForce = -5f + (float)rand.Next(11);
		float yForce = 80f + (float)rand.Next(10);
		if ((xForce < 0f && myTrans.position.x < 0f) || (xForce > 0f && myTrans.position.x > 0f))
		{
			xForce = 0f - xForce;
		}
		rb.constraints = (RigidbodyConstraints)120;
		while (myTrans.localPosition.y > -420f)
		{
			yield return stagePause.sync();
		}
		Sound.Instance.playSe(Sound.eSe.SE_216_sessyoku);
		Vector3 p = myTrans.localPosition;
		p.y = -420f;
		myTrans.localPosition = p;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.AddForce(new Vector3(xForce, yForce, 0f));
		yield return stagePause.sync();
		yield return stagePause.sync();
		while (rb.velocity.y > 0f)
		{
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(rb);
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.GetComponent<Renderer>().sharedMaterial = part.fadeMaterial;
		Mesh mesh = filter.sharedMesh;
		Color[] colors = mesh.colors;
		while (colors[0].a > 0f)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i].a -= Time.deltaTime * 5f;
				if (colors[i].a < 0f)
				{
					colors[i].a = 0f;
				}
			}
			mesh.colors = colors;
			yield return stagePause.sync();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
