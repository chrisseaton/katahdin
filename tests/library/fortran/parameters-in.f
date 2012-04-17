      PROGRAM TEST
      INTEGER X
      X = 14
      CALL SUB(X)
      END

      SUBROUTINE SUB(X)
      INTEGER X
      ASSERT X.EQ.14
      RETURN
      END
