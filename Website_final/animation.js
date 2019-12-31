function animate(){
x=x+1;
document.getElementById("ep1").style.height=x*2 +"px";
document.getElementById("ep1").style.marginBottom=100-x +"px";
document.getElementById("ep1").style.marginTop=100-x +"px";
document.getElementById("ep2").style.height=x*2 +"px";
document.getElementById("ep2").style.marginBottom=100-x +"px";
document.getElementById("ep2").style.marginTop=100-x +"px";
document.getElementById("her1").style.height=x*6 +"px";
document.getElementById("her1").style.marginBottom=300-x*3 +"px";
document.getElementById("her1").style.marginTop=300-x*3 +"px";
document.getElementById("her2").style.height=x*2 +"px";
document.getElementById("her2").style.marginBottom=100-x +"px";
document.getElementById("her2").style.marginTop=100-x +"px";
}
var z=1;
var x=0;
function init(){
	animate();
	while(z<100){
	setTimeout(animate, z*8);
	z++
	}
}
window.onload= init;