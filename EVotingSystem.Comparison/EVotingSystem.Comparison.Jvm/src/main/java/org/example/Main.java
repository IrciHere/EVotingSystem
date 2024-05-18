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
@OutputTimeUnit(TimeUnit.NANOSECONDS)
@Fork(1)
public class Main {
    private String _textForTests;
    private byte[] _aesKey;
    private byte[] _aesIv;

    @Param({"100", "500", "1000", "5000", "10000"})
    public int Size;

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
        
        _textForTests = Secret.Text.substring(0, Size);
    }
    
    @Benchmark
    public byte[] createHash() throws UnsupportedEncodingException, NoSuchAlgorithmException {
        byte[] hash = EncryptionService.hashSHA256(_textForTests);
        
        return hash;
    }
    
    @Benchmark
    public byte[] EncryptSecret() throws Exception {
        byte[] encrypted = EncryptionService.encryptSecret(_textForTests, _aesKey, _aesIv);
        
        return encrypted;
    }
    
    @Benchmark
    public String EncryptAndDecryptSecret() throws Exception {
        byte[] encrypted = EncryptionService.encryptSecret(_textForTests, _aesKey, _aesIv);
        String decrypted = EncryptionService.decryptSecret(encrypted, _aesKey, _aesIv);
        
        return decrypted;
    }
    
    public static void main(String[] args) throws IOException {
        org.openjdk.jmh.Main.main(args);
    }
}