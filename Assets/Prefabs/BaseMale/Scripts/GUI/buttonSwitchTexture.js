#pragma strict
@script RequireComponent(GUITexture);

var textureList : Texture[];
var currentTextureID : int = 0;

function Start () {

}

function Update () {
	if(Input.GetMouseButtonDown(0) && guiTexture.HitTest(Input.mousePosition)){
		currentTextureID ++;
		if(currentTextureID >= textureList.Length){
			currentTextureID = 0;
		}
		guiTexture.texture = textureList[currentTextureID];
	}
}