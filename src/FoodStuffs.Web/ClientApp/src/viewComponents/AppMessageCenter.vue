<template>
  <div
    v-if="messages.length > 0"
    :class="{messages: true, danger: messageIsError}"
    @click="clearMessages()"
  >
    <ul>
      <li
        v-for="message in messages"
        :key="message"
      >
        {{ message }}
      </li>
    </ul>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
  computed: {
    ...mapGetters('app', [
      'messages',
      'messageIsError',
    ]),
  },
  methods: {
    ...mapActions('app', [
      'clearMessages',
    ]),
  },
};
</script>

<style lang="scss" scoped>
@import "../style/theme";

.messages {
  position: relative;
  z-index: 1000;
  margin: 0;
  padding: 0;
  width: 100%;
  background-color: $color-success;
  box-shadow: $shadow;
  cursor: pointer;

  &.danger {
    background-color: $color-danger;
  }

  ul {
    padding: 1em 1em 1em 2.5rem;
    text-align: center;
    list-style: none;
    margin: 0;

    & > li {
      margin: 0 auto;
      max-width: ($contained-width * 0.8);
    }
  }
}
</style>
