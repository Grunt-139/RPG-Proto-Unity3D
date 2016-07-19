#pragma strict
@script RequireComponent(GUITexture)

var yPositionFromTop : boolean = true;
var xPositionFromRight : boolean = false;
var buttonPosition : Vector2;
var buttonSize : float = 1.0;
var getGuiTexture : GUITexture;

function Start () {
	getGuiTexture = GetComponent(GUITexture);
	transform.position = Vector3.zero;
}

function Update () {
	if(xPositionFromRight){
		guiTexture.pixelInset.x = Screen.width - buttonPosition.x - guiTexture.texture.width * .5 * buttonSize;
	}
	else{
		guiTexture.pixelInset.x = buttonPosition.x - guiTexture.texture.width * .5 * buttonSize;
	}
	if(yPositionFromTop){
		guiTexture.pixelInset.y = Screen.height - buttonPosition.y - guiTexture.texture.height * .5 * buttonSize;
	}
	else{
		guiTexture.pixelInset.y = buttonPosition.y - guiTexture.texture.height * .5 * buttonSize;
	}
	guiTexture.pixelInset.width = guiTexture.texture.width * buttonSize;
	guiTexture.pixelInset.height = guiTexture.texture.height * buttonSize;
}