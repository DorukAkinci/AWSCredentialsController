Sahip olduğunuz IAM Role'lere AssumeRole yaparak profile (credential) tanımı yaratır.

Örnek kullanım:
 
>***AWSCredentialsController.exe*** --profile ASSUME_ROLE_YAPACAK_YETKILI_PROFIL_ISMI --newprofile YENI_OLUSTURULACAK_PROFIL_ISMI --rolearn ROLE_ARN

Program için kullanilabilinecek komutlar:
-	  -l, --list                 Active profilleri listelemeyi saglar.
-     --np, --newprofile=VALUE
                             Yeni credential tanimi için profil adi
                               belirlemenizi saglar.
-     --ra, --rolearn=VALUE  Cross Role yapilacak IAM Role ARN
-     --sn, --sessionname=VALUE
                             STS Assume Role sirasinda atanacak session name
                               olarak kullanilir. ( Default:
                               AWSCredentialsController )
-     -p, --profile=VALUE        Amazon ortamina baglanmak için hazirda profile
                               tanimi varsa kullanabilmeyi saglar.
-     --ak, --accesskey=VALUE
                             Amazon ortamina baglanmak için kullanilacak
                               access key'i tanimlar.
-     --sk, --secretkey=VALUE
                             Amazon ortamina baglanmak için kullanilacak
                               secret key'i tanimlar.
-     -h, -?, --help             Bilgi almak için kullanilabilir.