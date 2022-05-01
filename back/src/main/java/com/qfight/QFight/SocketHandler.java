package com.qfight.QFight;

import java.io.IOException;
import java.util.List;
import java.util.ArrayList;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import org.springframework.stereotype.Component;
import org.springframework.web.socket.CloseStatus;
import org.springframework.web.socket.TextMessage;
import org.springframework.web.socket.WebSocketSession;
import org.springframework.web.socket.handler.TextWebSocketHandler;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import com.google.gson.reflect.TypeToken;
import java.lang.reflect.Type;

import java.util.concurrent.CopyOnWriteArrayList;

class Message<T> {
	public String type;
	public T data;

	Message(String type, T data) {
		this.type = type;
		this.data = data;
	}

	public <X> Message<X> toType(Class<X> castClass) {
		return new Message<X>(this.type, castClass.cast(data));
	}
}

class Coord {
	public String name;
	public float x;
	public float y;
	public float z;

	public String toString() {
		return String.format("%s@{Name: %s, x: %s, y: %s, z: %s}", getClass().getName(), name, x, y, z);
	}
}

@Component
public class SocketHandler extends TextWebSocketHandler {

	Logger logger = LoggerFactory.getLogger(SocketHandler.class);

	List<WebSocketSession> sessions = new CopyOnWriteArrayList<>();

	Type itemsMapType = new TypeToken<Message<Object>>() {
	}.getType();

	public static <T> T convertInstanceOfObject(Object o, Class<T> clazz) {
		try {
			return clazz.cast(o);
		} catch (ClassCastException e) {
			return null;
		}
	}

	@Override
	public void handleTextMessage(WebSocketSession session, TextMessage message)
			throws InterruptedException, IOException {
		logger.info("Prepare to get payload");
		try {
			Message<Object> unkownMessage = new Gson().fromJson(message.getPayload(), itemsMapType);

			logger.info(String.format("Received type %s", unkownMessage.type));

			if (unkownMessage.type.equals("update-coords")) {
				Type coordMapType = new TypeToken<Message<ArrayList<Coord>>>() {
				}.getType();
				Message<ArrayList<Coord>> coordMessage = new Gson().fromJson(message.getPayload(), coordMapType);
				for (Coord coord : coordMessage.data) {
					logger.info(String.format("Received coord %s", coord));
				}
				broadcastMessage(message, session.getId());
			} else if (unkownMessage.type.equals("run-script")) {
				logger.info(String.format("Received run script %s", unkownMessage.data));
				broadcastMessage(message, session.getId());
			}

		} catch (JsonSyntaxException e) {
			logger.info("Invalid payload");
			return;
		}
	}

	@Override
	public void afterConnectionEstablished(WebSocketSession session) throws Exception {
		// the messages will be broadcasted to all users.
		logger.info("Received connection " + session.getId());

		sessions.add(session);
	}

	@Override
	public void afterConnectionClosed(WebSocketSession session, CloseStatus status) throws Exception {
		sessions.remove(session);
	}

	private void broadcastMessage(TextMessage message, String excludeId) throws IOException {
		for (WebSocketSession webSocketSession : sessions) {
			logger.info(String.format("Send message to %s", webSocketSession.getId()));
			if (!webSocketSession.getId().equals(excludeId)) {
				webSocketSession.sendMessage(message);
			}
		}
	}
}