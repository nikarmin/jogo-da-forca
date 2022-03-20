/*
Projeto Final de INI14

Nicoli Ferreira 21689
Samuel de Campos Ferraz 21260
*/
#define led1  2
#define led2  3
#define led3  4
#define led4  5
#define led5  6
#define led6  7
#define led7  8
#define led8  9

void setup() {
  pinMode(led1, OUTPUT);
  pinMode(led2, OUTPUT);
  pinMode(led3, OUTPUT);
  pinMode(led4, OUTPUT);
  pinMode(led5, OUTPUT);
  pinMode(led6, OUTPUT);
  pinMode(led7, OUTPUT);
  pinMode(led8, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  if (Serial.available()){ //Verifica se a porta serial está aberta
    char c = Serial.read(); //Leitura do comando recebido do programa
    switch (c){ //Switch que verifica o comando recebido
      case 'A':
        digitalWrite(led1, HIGH);
        Serial.println("Errou 1 vez"); //Envia um texto que será escrito em um textbox do jogo
        break;
      case 'B':
        digitalWrite(led2, HIGH);
        Serial.println("Errou 2 vezes");
        break;  
      case 'C':
        digitalWrite(led3, HIGH);
        Serial.println("Errou 3 vezes");
        break; 
      case 'D':
        digitalWrite(led4, HIGH);
        Serial.println("Errou 4 vezes");
        break;    
      case 'E':
        digitalWrite(led5, HIGH);
        Serial.println("Errou 5 vezes");
        break; 
      case 'F':
        digitalWrite(led6, HIGH);
        Serial.println("Errou 6 vezes");
        break;
      case 'G':
        digitalWrite(led7, HIGH);
        Serial.println("Errou 7 vezes");
        break;
      case 'H':
        Serial.println("Errou 8 vezes");
        Serial.println("GAME OVER!");
        for (;;){ //Fica piscando os leds quando a pessoa perde
          digitalWrite(led1, LOW);
          digitalWrite(led2, LOW);
          digitalWrite(led3, LOW);
          digitalWrite(led4, LOW);
          digitalWrite(led5, LOW);
          digitalWrite(led6, LOW);
          digitalWrite(led7, LOW);
          digitalWrite(led8, LOW);
          delay(500);
          digitalWrite(led1, HIGH);
          digitalWrite(led2, HIGH);
          digitalWrite(led3, HIGH);
          digitalWrite(led4, HIGH);
          digitalWrite(led5, HIGH);
          digitalWrite(led6, HIGH);
          digitalWrite(led7, HIGH);
          digitalWrite(led8, HIGH);
          delay(500);
          c = Serial.read(); //Lê se algum foi recebido
          if (c == 'I'){ //Se o comando de reiniciar for enviado os leds apagam para se jogar novamente
            digitalWrite(led1, LOW);
            digitalWrite(led2, LOW);
            digitalWrite(led3, LOW);
            digitalWrite(led4, LOW);
            digitalWrite(led5, LOW);
            digitalWrite(led6, LOW);
            digitalWrite(led7, LOW);
            digitalWrite(led8, LOW);
            break;
          }
       }
      case 'I': //Quando o jogo é fechado o comando I é enviado e os leds se apagam
        digitalWrite(led1, LOW);
        digitalWrite(led2, LOW);
        digitalWrite(led3, LOW);
        digitalWrite(led4, LOW);
        digitalWrite(led5, LOW);
        digitalWrite(led6, LOW);
        digitalWrite(led7, LOW);
        digitalWrite(led8, LOW);
        break;
    }
 }
}
