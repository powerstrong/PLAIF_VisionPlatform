# ssh 키 생성
ssh-keygen -b 4096 -t rsa -f ./id_rsa -q -N "''"

# ssh 키 복사
cat ./id_rsa.pub | ssh yhpark@192.168.1.75 "cat >> .ssh/authorized_keys"

# 파일 가져오기
scp yhpark@192.168.1.75:~/catkin_ws/config/config_file/zivid_setting2.yml .

# 지우기
# ssh-keygen -R 192.168.1.75 
