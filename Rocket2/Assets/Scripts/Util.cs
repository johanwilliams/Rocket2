using UnityEngine;

public class Util {

    // Recursive utility method which assigns the incoming game object and all its children to a layer
    public static void SetLayerRecursively(GameObject _obj, int _newLayer) {
        if (_obj = null)
            return;

        _obj.layer = _newLayer;

        foreach(Transform _child in _obj.transform) {
            if (_child == null)
                continue;
            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }


}
