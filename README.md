# Legend Of Archer
<img width="1051" height="587" alt="image" src="https://github.com/user-attachments/assets/32c834e2-ca86-49a7-8b11-94904c6dc05e" />


## 📖 목차
1. [프로젝트 소개](#프로젝트-소개)
2. [팀 소개](#팀-소개)
3. [게임 소개](#게임-소개)
4. [기술 스택](#기술-스택)
5. [스크립트 설명](#스크립트-설명)


## 프로젝트 소개
팀스파르타 내일배움캠프 Unity 11기
'궁수의 전설'을 모티브로 한 탑다운 로그라이크 형식의 슈팅 게임
![123](https://github.com/user-attachments/assets/be06cd6e-75de-40c3-875e-997feba4e29e)

## 팀 소개
https://github.com/SAjA-Bo2jo/LegendOfArcher/graphs/contributors
<br />
* 주용진 - 게임 매니저 작성 및 통합 기능
* 김인빈 - UI, 게임 시스템
* 손건희 - 맵, 연출, 최적화 
* 오경석 - 플레이어, 전투 시스템
* 조민석 - 적과 보스 AI

## 게임 소개
### 타이틀 화면
<img width="1051" height="587" alt="image" src="https://github.com/user-attachments/assets/95200e6e-c0ed-4f36-be9b-062439da8d26" />

1. Game Start 버튼을 통해 게임을 플레이할 수 있습니다.
2. Character 버튼을 통해 캐릭터 커스터마이징을 할 수 있습니다.(현재 미구현)
3. Option 버튼을 통해 옵션 UI를 띄울 수 있습니다.
4. Exit 버튼을 통해 게임을 종료할 수 있습니다.

### 메인 게임 화면
<img width="1059" height="593" alt="image" src="https://github.com/user-attachments/assets/192bf0bd-578a-4bbe-8e13-b208ddcde25a" />
WASD와 방향키로 캐릭터를 움직일 수 있고, 캐릭터가 움직이지 않으면 적을 향해 자동으로 화살을 발사합니다.
메인 게임 UI를 통해 현재 층수, 능력치, 현재 체력 등을 확인할 수 있습니다.
<br /> <br />
<img width="1050" height="587" alt="image" src="https://github.com/user-attachments/assets/05d15958-ea97-45d7-b6cf-30ebd6e177a0" />
5개의 스테이지마다 보스 몬스터가 등장합니다.
<br /> <br />

### 결과 화면
<img width="1052" height="585" alt="image" src="https://github.com/user-attachments/assets/b524b256-664a-4d81-b69f-d4db82af418d" />
준비된 스테이지를 모두 클리어한 경우 다음과 같은 결과 화면이 등장합니다.
<br /> <br />
<img width="1047" height="582" alt="image" src="https://github.com/user-attachments/assets/84addc17-5263-4aa1-a9ce-625e94eb42a4" />
플레이 도중 몬스터나 함정에 데미지를 받아 플레이어가 사망한 경우에는 다음과 같은 결과 화면과 플레이어를 죽인 몬스터의 이미지가 출력됩니다.
<br /><br />

## 기술 스택

### 언어
* C#

### 개발 환경
* VisualStudio
* JetBrains Rider

### 게임 엔진
* Unity - 2022.3.17f1
<br /> <br />
## 스크립트 설명

### Object Pool
화살과 같은 몇몇 오브젝트들은 게임 내에서 매우 빈번하게 생성 및 파괴됩니다. 이 과정에서 Instantiate 메소드와 Destroy 메소드는 메모리에서 상당한 비용을 요구합니다.
따라서 생성과 파괴가 빈번하게 발생하는 오브젝트를 오브젝트 풀링을 통해 미리 생성한 후 필요할 때마다 활성화하는 식으로 메모리의 비용을 줄일 수 있습니다.

### 제네릭 싱글톤
게임을 제작하면서 매니저 클래스들이 점점 더 많이 요구되었습니다. 이에 매니저 스크립트들을 각각 싱글톤 패턴을 추가하여 이들을 관리하고자 하였지만, 이는 꽤나 번거롭고 유지보수성이 떨어지는 방식입니다. 
이에 싱글톤 패턴을 매니저 스크립트들에게 상속시켜서 한번에 싱글톤 패턴을 포함시켜주는 제네릭 싱글톤 클래스를 작성하였습니다.
