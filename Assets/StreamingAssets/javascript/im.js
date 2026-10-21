============================================================
Script:     Instant Image Zooming

Functions:  This script can zoom-in/zoom-out any image in
            your page by clicking on the image.

Browsers:   NS6+, IE.all (NS4 degrades gracefully)

Author:     etLux
============================================================

INSTRUCTIONS:

Step 1.

Put the following script in the <head>...</head> section of
your page:

<script>
// C.2004 by CodeLifter.com
var nW,nH,oH,oW;
function zoomToggle(iWideSmall,iHighSmall,iWideLarge,iHighLarge,whichImage){
oW=whichImage.style.width;oH=whichImage.style.height;
if((oW==iWideLarge)||(oH==iHighLarge)){
nW=iWideSmall;nH=iHighSmall;}else{
nW=iWideLarge;nH=iHighLarge;}
whichImage.style.width=nW;whichImage.style.height=nH;
}
</script>

Step 2.

Attach the function to your image with an onclick event, as
shown below:

<img
border="0" 
src="yourimage.gif" 
width="100"
height="100"
onclick="zoomToggle('100px','100px','200px','200px',this);"
>

Set the variables in the function call:

1st is the small width
2nd is the small height
3rd is the large width
4th is the large height

(The 'this' at the end should not be changed.)

FAQS:

You *can* use this on more than one image in the page.
Just attach the function to each image, as per the example
code above.

The starting size of the image as set with width= and
height= in the <img etc> tag and the small width and 
height in the function *may* be different if you wish.
View-source on the popup demo on this page for an example;
see the second image instance in the demo.

Cautionary Note:

Be careful to observer the syntax in the function call
exactly as shown, including the single ' quote marks and 
the px after each dimension!

============================================================
