package org.example;

import org.openjdk.jmh.annotations.*;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.security.NoSuchAlgorithmException;
import java.util.concurrent.TimeUnit;

//TIP To <b>Run</b> code, press <shortcut actionId="Run"/> or
// click the <icon src="AllIcons.Actions.Execute"/> icon in the gutter.
@State(Scope.Thread)
@BenchmarkMode(Mode.AverageTime)
@OutputTimeUnit(TimeUnit.MILLISECONDS)
public class Main {
    private static final String Password = "TestPassw0rd%";
    private static final String Secret = "TestSecr3t@";
    private byte[] _aesKey;
    private byte[] _aesIv;

    @Setup
    public void setup() {
        _aesKey = new byte[32];
        for (int i = 0; i < 32; i++) {
            _aesKey[i] = (byte) (i * 3);
        }

        _aesIv = new byte[16];
        for (int i = 0; i < 16; i++) {
            _aesIv[i] = (byte) (i * 4 + 7);
        }
    }
    
    @Benchmark
    public byte[] createHash() throws UnsupportedEncodingException, NoSuchAlgorithmException {
        byte[] hash = EncryptionService.hashSHA256(Password);
        
        return hash;
    }
    
    @Benchmark
    public byte[] EncryptSecret() throws Exception {
        byte[] encrypted = EncryptionService.encryptSecret(Secret, _aesKey, _aesIv);
        
        return encrypted;
    }
    
    @Benchmark
    public String EncryptAndDecryptSecret() throws Exception {
        byte[] encrypted = EncryptionService.encryptSecret(Secret, _aesKey, _aesIv);
        String decrypted = EncryptionService.decryptSecret(encrypted, _aesKey, _aesIv);
        
        return decrypted;
    }
    
    public static void main(String[] args) throws IOException {
        org.openjdk.jmh.Main.main(args);
    }
}