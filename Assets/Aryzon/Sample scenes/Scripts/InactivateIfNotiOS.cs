using UnityEngine;

public class InactivateIfNotiOS : MonoBehaviour {

	void Awake () {
		#if !PLATFORM_IOS
            gameObject.SetActive(false);
        #endif
	}
	
}
