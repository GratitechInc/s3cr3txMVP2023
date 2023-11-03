    function generateCaptcha()
         {
             var alpha = new Array('A','B','C','D','E','F','G','H','I','J','K','L','M',
                                   'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                   'a','b','c','d','e','f','g','h','i','j','k','l','m',
                                   'n','o','p','q','r','s','t','u','v','w','x','y','z');
             var i;
             for (i=0;i<4;i++){
               var a = alpha[Math.floor(Math.random() * alpha.length)];
               var b = alpha[Math.floor(Math.random() * alpha.length)];
               var c = alpha[Math.floor(Math.random() * alpha.length)];
               var d = alpha[Math.floor(Math.random() * alpha.length)];
              }
            var code = a + '' + b + '' + '' + c + '' + d;
            document.getElementById("mainCaptcha").value = code
          }
          function CheckValidCaptcha(){
              var string1 = removeSpaces(document.getElementById('mainCaptcha').value);
              var string2 = removeSpaces(document.getElementById('txtCaptchaInput').value);
              if (string1 == string2){
                  document.getElementById('success').innerHTML = "Form is validated Successfully";
                  document.getElementById('error').innerHTML = "";
                  document.getElementById('btnSubmit').setAttribute('disabled', 'false');
                  document.getElementById('btnSubmit').removeAttribute('disabled');
                  

         //alert("Form is validated Successfully");
                return true;
              }
              else{
                  document.getElementById('error').innerHTML = "Please enter a valid captcha.";
                  document.getElementById('success').innerHTML = "";

         //alert("Please enter a valid captcha.");
                return false;
              }
          }
          function removeSpaces(string){
            return string.split(' ').join('');
          }
 

