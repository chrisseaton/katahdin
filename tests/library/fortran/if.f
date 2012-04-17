      PROGRAM TEST
      
      INTEGER X
      
      X = 0
      IF (X.EQ.0) X = 1
      ASSERT X.EQ.1
      
      X = 0
      IF (X.EQ.1) X = 1
      ASSERT X.EQ.0
      
      END
