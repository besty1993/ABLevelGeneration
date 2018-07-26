using UnityEngine;
using System.Collections;

public class ABTNT : ABGameObject {

	public float _explosionArea = 1f;
	public float _explosionPower = 1f;
	public float _explosionDamage = 1f;
	private bool _exploded = false;
    private static int _countExplode = 0;

	public override void Die(bool withEffect = true)
	{		
		//ScoreHud.Instance.SpawnScorePoint(200, transform.position);
		if (!_exploded) {
			_exploded = true;
			Explode (transform.position, _explosionArea, _explosionPower, _explosionDamage, gameObject);
		}

		base.Die (withEffect);
	}

    public static void ResetCountExplode() {
        _countExplode = 0;
    }

	public static void Explode(Vector2 position, float explosionArea, float explosionPower, float explosionDamage, GameObject explosive) {
		Collider2D[] colliders = Physics2D.OverlapCircleAll (position, explosionArea);
		foreach (Collider2D coll in colliders) {

			if (coll.attachedRigidbody && coll.gameObject != explosive && coll.GetComponent<ABBird>() == null) {

				float distance = Vector2.Distance ((Vector2)coll.transform.position, position);
				Vector2 direction = ((Vector2)coll.transform.position - position).normalized;

				ABGameObject abGameObj = coll.gameObject.GetComponent<ABGameObject> ();
				if(abGameObj)
					coll.gameObject.GetComponent<ABGameObject> ().DealDamage (explosionDamage/distance);
				
				coll.attachedRigidbody.AddForce (direction * (explosionPower / (distance * 2f)), ForceMode2D.Impulse);
			}

            ABLevel abLevel = LevelList.Instance.GetCurrentLevel();

            if (coll.gameObject.name == "TNT(Clone)" && coll.gameObject.tag=="Untagged" && abLevel.isTestTriggerPoint)
            {
                
                _countExplode++;

                print("Check outloop TNT Explode " + abLevel.countTNT + " , " + _countExplode);
                if (abLevel.countTNT == _countExplode) {
                    abLevel.isTNTExplode = true;
                    print("----");
                    print("Check inloop TNT Explode " + abLevel.countTNT + " , " + _countExplode);
                    print("TNT Explode True");
                    _countExplode = 0;
                }
            }
		}
	}
}
