using UnityEngine;

public class InactivateIfNotAndroid : MonoBehaviour {

	void Awake () {
		#if !PLATFORM_ANDROID
            gameObject.SetActive(false);
        #endif
	}
	
}
