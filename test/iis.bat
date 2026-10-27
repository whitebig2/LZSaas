C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_basic

C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_clients

C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_company

C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_customer

C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_gateway

C:\Windows\System32\inetsrv\appcmd.exe stop apppool /apppool.name:lz_users




C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_basic

C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_clients

C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_company

C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_customer

C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_gateway

C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:lz_users

curl http://localhost:8862/health
curl http://localhost:8860/health
curl http://localhost:8861/health
curl http://localhost:8862/health
curl http://localhost:8863/health
curl http://localhost:8864/health
curl http://localhost:8867/health

