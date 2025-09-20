CREATE TABLE professor (
  id INT IDENTITY PRIMARY KEY,
  name NVARCHAR(100) NOT NULL
);

CREATE TABLE subject (
  id INT IDENTITY PRIMARY KEY,
  title NVARCHAR(200) NOT NULL,
  credits INT NOT NULL,
  professor_id INT NOT NULL,
  CONSTRAINT fk_subject_professor FOREIGN KEY (professor_id) REFERENCES professor(id)
);

CREATE TABLE student (
  id INT IDENTITY PRIMARY KEY,
  first_name NVARCHAR(100) NOT NULL,
  last_name NVARCHAR(100) NOT NULL
);

CREATE TABLE enrollment (
  id INT IDENTITY PRIMARY KEY,
  studentId INT NOT NULL,
  subjectId INT NOT NULL,
  createdat DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT fk_enrollment_student FOREIGN KEY (studentId) REFERENCES student(id),
  CONSTRAINT fk_enrollment_subject FOREIGN KEY (subjectId) REFERENCES subject(id),
  CONSTRAINT uq_student_subject UNIQUE (studentId, subjectId)
);

INSERT INTO professor (name) VALUES ('Andres'), ('Camila'), ('Carlos'), ('Maria'), ('Pablo'); 
INSERT INTO subject (title, credits, professor_id) VALUES ('Matematicas 1',3,1), ('Fisica 1',3,1), ('Programación 1',3,2), ('Bases de Datos 1',3,2), ('Matemáticas 2',3,3), ('Fisica 2',3,3), ('Programación 2',3,4), ('Bases de Datos 2',3,4), ('Redes 1',3,5), ( 'Redes 2',3,5);
